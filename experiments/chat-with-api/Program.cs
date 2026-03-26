using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;
using Microsoft.Extensions.DependencyInjection;
using chat_with_api.Plugins;
using chat_with_api.Services;
using chat_with_api.State;

var builder = Kernel.CreateBuilder();

// 7b+ para tool calling confiável
// HttpClient via IHttpClientFactory
builder.Services.AddHttpClient(string.Empty, client =>
{
    client.Timeout = TimeSpan.FromMinutes(10);
});

builder.AddOllamaChatCompletion(
    modelId: "llama3.1:8b-instruct-q4_K_M",
    endpoint: new Uri("http://localhost:11434")
);

builder.Services.AddSingleton<DeliveryApiService>();
builder.Services.AddSingleton<PedidoState>();

var kernel = builder.Build();
kernel.ImportPluginFromType<DeliveryPlugin>();

var chat = kernel.GetRequiredService<IChatCompletionService>();
var state = kernel.GetRequiredService<PedidoState>();

// Usar settings tipado para Ollama — sem ExtensionData frágil
var settings = new OllamaPromptExecutionSettings
{
    Temperature = 0.1f,
    NumPredict = 512,   // equivalente ao max_tokens no Ollama
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
};

var history = new ChatHistory();
history.AddSystemMessage(BuildSystemPrompt());

// Banner
RenderBanner();
Console.WriteLine("Bem-vindo ao TechBot! Como posso te ajudar?");

while (true)
{
    Console.Write("\n> ");
    var input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input)) continue;
    if (input.Trim().ToLower() == "sair") break;

    history.AddUserMessage(input);
    TrimHistory(history, maxTurns: 10);

    try
    {
        var response = await chat.GetChatMessageContentAsync(history, settings, kernel);
        var content = response.Content ?? "(sem resposta)";

        history.AddAssistantMessage(content);

        // Debug opcional — remova em produção
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"[etapa: {state.EtapaAtual}]");
        Console.ResetColor();

        Console.WriteLine(content);
    }
    catch (HttpRequestException ex)
    {
        Console.WriteLine($"[Erro de conexão com Ollama]: {ex.Message}");
    }
    catch (TaskCanceledException)
    {
        Console.WriteLine("[Timeout]: O modelo demorou demais. Tente novamente.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[Erro inesperado]: {ex.Message}");
    }
}

// ── helpers ────────────────────────────────────────────────────────────────

static void TrimHistory(ChatHistory history, int maxTurns)
{
    // Preserva sempre o system message (índice 0)
    // Remove pares (user + assistant) mais antigos quando passa do limite
    // Cada "turno" = 2 mensagens (user + assistant)
    int maxMessages = (maxTurns * 2) + 1; // +1 para o system

    if (history.Count > maxMessages)
    {
        // Remove a partir do índice 1 (preserva system prompt)
        int toRemove = history.Count - maxMessages;
        history.RemoveRange(1, toRemove);
    }
}

static string BuildSystemPrompt() => """
    Você é TechBot, atendente virtual de delivery.

    ## IDENTIDADE
    - Seja simpático, direto e natural
    - Nunca mencione funções, ferramentas ou termos técnicos
    - Nunca invente produtos, preços ou dados

    ## FLUXO OBRIGATÓRIO (siga esta ordem)
    1. Se não tiver telefone → peça o telefone
    2. Se tiver telefone mas não tiver itens → pergunte o que deseja pedir
    3. Se o cliente quiser ver o cardápio → liste os produtos disponíveis
    4. Se o cliente escolher um produto → adicione ao pedido e confirme
    5. Se tiver itens mas não tiver endereço → peça o endereço
    6. Se tiver endereço mas não tiver pagamento → pergunte a forma de pagamento
    7. Se tudo preenchido → confirme o resumo e finalize

    ## REGRAS DE TOOL
    - Sempre que o cliente mencionar um produto pelo nome → chame BuscarProdutos
    - Sempre que o cliente quiser ver opções → chame ListarProdutos
    - Sempre registre as informações nas funções corretas
    - Nunca pergunte algo que já foi informado

    ## FORMATO
    - Respostas curtas (1-3 frases)
    - Uma pergunta por mensagem
    - Tom amigável e profissional
    """;

static void RenderBanner()
{
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
}