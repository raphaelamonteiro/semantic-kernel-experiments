using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.Extensions.DependencyInjection;
using Plugins;

// cria o builder do kernel
var builder = Kernel.CreateBuilder();

builder.AddOllamaChatCompletion(
    modelId: "qwen2.5:3b",
    endpoint: new Uri("http://localhost:11434")
);

// aumenta timeout
builder.Services.AddHttpClient("ollama", client =>
{
    client.Timeout = TimeSpan.FromMinutes(10);
});

// construindo o Kernel
var kernel = builder.Build();

// registra o plugin e transforma as funções em tools para o LLM
kernel.ImportPluginFromType<DeliveryPlugin>();

// serviço de conversa com o LLM
var chat = kernel.GetRequiredService<IChatCompletionService>();

// guarda toda a conversa
var history = new ChatHistory();
history.AddSystemMessage("""
Você é um atendente de delivery virtual.

Funções principais:

1. Mostrar apenas produtos disponíveis do sistema.
2. Ajudar o cliente a escolher itens corretamente, fornecendo informações precisas (preço, descrição, variações).
3. Registrar pedidos confirmando quantidade, variações e endereço de entrega.

Regras obrigatórias:

- Use sempre as funções do sistema para acessar dados.
- Não invente produtos, preços ou detalhes.
- Seja cortês e confirme o pedido antes de finalizar.
- Se houver dúvida sobre disponibilidade, consulte o sistema em vez de adivinhar.
""");

var settings = new PromptExecutionSettings()
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
};

Console.WriteLine("Controle de Luzes iniciado.");
Console.WriteLine("Digite um comando ou 'sair' para encerrar.");

while (true)
{
    Console.Write("\n> ");
    var input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input))
        continue;

    if (input.ToLower() == "sair")
        break;

    // adiciona pergunta do usuário ao histórico
    history.AddUserMessage(input);

    // chama o modelo, agora com tratamento de erros
    try
    {
        var response = await chat.GetChatMessageContentAsync(
            history,
            settings,
            kernel
        );

        // adiciona resposta ao histórico e mostra na tela
        history.AddAssistantMessage(response.Content ?? "");
        Console.WriteLine(response.Content);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro ao chamar o modelo: {ex.Message}");
    }
}