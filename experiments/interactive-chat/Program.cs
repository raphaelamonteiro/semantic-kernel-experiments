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
kernel.ImportPluginFromType<LightsPlugin>();

// serviço de conversa com o LLM
var chat = kernel.GetRequiredService<IChatCompletionService>();

// guarda toda a conversa
var history = new ChatHistory();

// define o comportamento da IA
history.AddSystemMessage("""
Você controla as luzes de uma casa.

Existem três luzes:
1 - Sala
2 - Cozinha
3 - Quarto

Você pode:
- listar as luzes
- ligar uma luz
- desligar uma luz

Use as funções disponíveis sempre que necessário.
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

    // chama o modelo
    var response = await chat.GetChatMessageContentAsync(
        history,
        settings,
        kernel
    );

    // adiciona resposta ao histórico
    history.AddAssistantMessage(response.Content ?? "");
    // mostra a resposta
    Console.WriteLine(response.Content);
}