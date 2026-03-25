using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.Extensions.DependencyInjection;
using chat_with_api.Plugins;
using chat_with_api.Services;
using chat_with_api.State;

// cria o HttpClient 
var httpClient = new HttpClient
{
    Timeout = TimeSpan.FromMinutes(10)
};

// cria o builder do kernel
var builder = Kernel.CreateBuilder();

builder.AddOllamaChatCompletion(
    modelId: "qwen2.5:3b",
    endpoint: new Uri("http://localhost:11434")
);

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
- Nunca mencione funções ou nomes técnicos
- Nunca peça para o usuário digitar comandos

FUNÇÕES:
- Use funções automaticamente quando necessário
- Nunca explique que está usando funções

FLUXO:
1. Solicitar telefone
2. Escolher produtos
3. Solicitar endereço
4. Solicitar pagamento
5. Finalizar pedido

COMPORTAMENTO:
- Respostas curtas
- Um passo por vez
- Seja natural e direto
""");

var settings = new PromptExecutionSettings
{
    ExtensionData = new Dictionary<string, object>
    {
        ["temperature"] = 0.3,
        ["max_tokens"] = 150
    },
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
};

Console.ForegroundColor = ConsoleColor.Red;

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

    // mantém só as 10 ultimas interações
    if (history.Count > 10)
    {
        history.RemoveRange(0, history.Count - 10);
    }

    history.AddUserMessage(input);

    // chama o modelo COM tratamento de erro
    try
    {
        var response = await chat.GetChatMessageContentAsync(
            history,
            settings,
            kernel
        );

        var content = response.Content ?? "";

        if (content.Contains("<tool_response>"))
        {
            content = content
                .Replace("<tool_response>", "")
                .Replace("</tool_response>", "")
                .Trim();
        }

        Console.WriteLine($"[DEBUG] Etapa atual: {kernel.GetRequiredService<PedidoState>().EtapaAtual}");

        history.AddAssistantMessage(content);
        Console.WriteLine(content);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro ao chamar o modelo: {ex.Message}");
    }
}