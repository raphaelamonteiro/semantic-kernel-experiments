using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.Extensions.DependencyInjection;
using chat_with_api.Plugins;
using chat_with_api.Services;
using chat_with_api.State;

// cria o builder do kernel
var builder = Kernel.CreateBuilder();

builder.AddOllamaChatCompletion(
    modelId: "qwen2.5:3b",
    endpoint: new Uri("http://localhost:11434")
);

builder.Services.AddHttpClient("ollama", client =>
{
    client.Timeout = TimeSpan.FromMinutes(10);
});

// 🔥 ADICIONA ISSO
builder.Services.AddSingleton<DeliveryApiService>();
builder.Services.AddSingleton<PedidoState>();

var kernel = builder.Build();

// registra o plugin e transforma as funções em tools para o LLM
kernel.ImportPluginFromType<DeliveryPlugin>();

// serviço de conversa com o LLM
var chat = kernel.GetRequiredService<IChatCompletionService>();

// guarda toda a conversa
var history = new ChatHistory();
history.AddSystemMessage("""
Você é um atendente de delivery chamado TechBot.

REGRAS:

- Nunca invente produtos ou preços
- Sempre use as funções disponíveis para buscar dados
- Nunca mencione funções ao usuário

FLUXO:

- Peça telefone antes do pedido
- Depois permita escolher produtos
- Depois peça endereço
- Depois forma de pagamento

COMPORTAMENTO:

- Respostas curtas
- Um passo por vez
""");

var settings = new PromptExecutionSettings()
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
};

Console.ForegroundColor = ConsoleColor.Cyan;

Console.WriteLine(@"
+==========================================================================+
| _________  _______   ________  ___  ___  ________  ________  _________   |
||\___   ___\\  ___ \ |\   ____\|\  \|\  \|\   __  \|\   __  \|\___   ___\ |
|\|___ \  \_\ \   __/|\ \  \___|\ \  \\\  \ \  \|\ /\ \  \|\  \|___ \  \_| |
|     \ \  \ \ \  \_|/_\ \  \    \ \   __  \ \   __  \ \  \\\  \   \ \  \  |
|      \ \  \ \ \  \_|\ \ \  \____\ \  \ \  \ \  \|\  \ \  \\\  \   \ \  \ |
|       \ \__\ \ \_______\ \_______\ \__\ \__\ \_______\ \_______\   \ \__\|
|        \|__|  \|_______|\|_______|\|__|\|__|\|_______|\|_______|    \|__||
+==========================================================================+
");

Console.ResetColor();

Console.WriteLine("Bem-vindo ao TechBot");
Console.WriteLine("Faça seu pedido ou digite 'sair' para encerrar o atendimento.");

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
        var content = response.Content ?? "";

        // remove possíveis tool responses vazando
        if (content.Contains("<tool_response>"))
        {
            content = content
                .Replace("<tool_response>", "")
                .Replace("</tool_response>", "")
                .Trim();
        }

        Console.WriteLine(content);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro ao chamar o modelo: {ex.Message}");
    }
}