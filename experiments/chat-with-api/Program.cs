using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;
using Microsoft.Extensions.DependencyInjection;
using chat_with_api.Plugins;
using chat_with_api.Services;
using chat_with_api.State;
using chat_with_api.ToolCall;

var builder = Kernel.CreateBuilder();

builder.Services.AddHttpClient(string.Empty, client =>
{
    client.Timeout = TimeSpan.FromMinutes(10);
});

builder.AddOllamaChatCompletion(
    modelId: "qwen2.5:3b",
    endpoint: new Uri("http://localhost:11434")

);


builder.Services.AddSingleton<DeliveryApiService>();
builder.Services.AddSingleton<PedidoState>();

var kernel = builder.Build();


kernel.ImportPluginFromType<DeliveryPlugin>();
//kernel.FunctionInvocationFilters.Add(new ToolCallLogger());

var chat = kernel.GetRequiredService<IChatCompletionService>();
var state = kernel.GetRequiredService<PedidoState>();

var settings = new OllamaPromptExecutionSettings
{
    Temperature = 0.0f,
    NumPredict = 300,
    TopK = 20,
    TopP = 0.8f,
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
    ExtensionData = new Dictionary<string, object>
    {
        ["num_ctx"] = 2048
    }
};

var history = new ChatHistory();
history.AddSystemMessage(BuildSystemPrompt());

RenderBanner();
Console.WriteLine("Bem-vindo ao TechBot! Como posso te ajudar?");

while (true)
{
    Console.Write("\n> ");
    var input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input)) continue;
    if (input.Trim().ToLower() == "sair") break;

    TrimHistory(history, maxTurns: 6);
    history.AddUserMessage(input);

    using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));

    try
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("Processando");

        var responseTask = chat.GetChatMessageContentAsync(
            history, settings, kernel, cts.Token);

        while (!responseTask.IsCompleted)
        {
            Console.Write(".");
            await Task.Delay(500);
        }

        Console.WriteLine();
        Console.ResetColor();

        var response = await responseTask;
        var content = response.Content ?? "(sem resposta)";

        history.AddAssistantMessage(content);

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"[etapa: {state.EtapaAtual}] | telefone: {state.Telefone ?? "vazio"} | itens: {state.Itens.Count}");
        Console.ResetColor();

        Console.WriteLine(content);
    }
    catch (OperationCanceledException)
    {
        Console.ResetColor();
        Console.WriteLine("\n[Timeout]: O modelo demorou demais. Tente novamente.");
        if (history.Count > 0 && history.Last().Role == AuthorRole.User)
            history.RemoveAt(history.Count - 1);
    }
    catch (HttpRequestException ex)
    {
        Console.ResetColor();
        Console.WriteLine($"[Erro de conexão]: {ex.Message}");
    }
    catch (Exception ex)
    {
        Console.ResetColor();
        Console.WriteLine($"[Erro]: {ex.Message}");
    }
}

static void TrimHistory(ChatHistory history, int maxTurns)
{
    int maxMessages = (maxTurns * 2) + 1;
    if (history.Count > maxMessages)
        history.RemoveRange(1, history.Count - maxMessages);

    // Garante que após o system, o próximo é sempre User
    while (history.Count > 1 && history[1].Role != AuthorRole.User)
        history.RemoveAt(1);
}

static string BuildSystemPrompt() => """
    Você é TechBot, atendente virtual de delivery.

    ## IDENTIDADE
    - Seja simpático, direto e natural
    - NUNCA invente produtos, preços, categorias ou dados
    - NUNCA responda sobre produtos sem consultar o cardápio

    ## FLUXO OBRIGATÓRIO
    1. Sem telefone → peça o telefone e chame InformarTelefone
    2. Com telefone, sem itens → pergunte o que deseja
    3. Cliente quer ver cardápio → chame ListarProdutos
    4. Cliente menciona produto → chame BuscarProdutos, depois AdicionarItemPedido
    5. Com itens, sem endereço → peça endereço e chame InformarEndereco
    6. Com endereço, sem pagamento → peça pagamento e chame InformarPagamento
    7. Tudo preenchido → chame VerPedido para mostrar resumo, depois FinalizarPedido

    ## REGRAS CRÍTICAS
    - Só mostre produtos que vieram das funções — jamais invente
    - Só mostre preços que vieram das funções — jamais invente
    - Sempre chame a função antes de responder sobre qualquer dado
    - Para mensagens simples como "obrigado", "ok", "sim" → responda direto, sem chamar funções

    ## FORMATO
    - Respostas curtas (1-3 frases)
    - Uma ação por vez
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
