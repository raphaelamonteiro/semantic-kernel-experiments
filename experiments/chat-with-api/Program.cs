using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
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
    modelId: "qwen2.5:7b",
    endpoint: new Uri("http://localhost:11434")
);

builder.Services.AddSingleton<DeliveryApiService>();
builder.Services.AddSingleton<PedidoState>();

var kernel = builder.Build();
kernel.ImportPluginFromType<DeliveryPlugin>();
kernel.FunctionInvocationFilters.Add(new ToolCallLogger()); // ← essencial para debug

var chat = kernel.GetRequiredService<IChatCompletionService>();
var state = kernel.GetRequiredService<PedidoState>();

var settings = new OllamaPromptExecutionSettings
{
    Temperature = 0.0f,
    NumPredict = 150,        // era 300 — respostas curtas, menos tokens = menos espera
    TopK = 10,               // era 20 — menos candidatos = decisão mais rápida
    TopP = 0.85f,
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(
    autoInvoke: true,
    options: new FunctionChoiceBehaviorOptions
    {
        AllowConcurrentInvocation = false,
        AllowParallelCalls = false
    }
),
    ExtensionData = new Dictionary<string, object>
    {
        ["num_ctx"] = 2048   // era 1536 — mais folgado, mas não exagera na CPU
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

    TrimHistory(history, maxTurns: 4);
    history.AddUserMessage(input);

    using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));


    try
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("Processando");
        var sw = System.Diagnostics.Stopwatch.StartNew();

        var sb = new StringBuilder();

        await foreach (var chunk in chat.GetStreamingChatMessageContentsAsync(
            history, settings, kernel, cts.Token))
        {
            if (!string.IsNullOrEmpty(chunk.Content))
            {
                // Primeiro chunk de texto: quebra a linha do "Processando..."
                if (sb.Length == 0)
                {
                    Console.WriteLine($" ({sw.Elapsed.TotalSeconds:F1}s)");
                    Console.ResetColor();
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine($"[etapa: {state.EtapaAtual}]");
                    Console.ResetColor();
                }
                sb.Append(chunk.Content);
                Console.Write(chunk.Content); // imprime em tempo real
            }
            else
            {
                Console.Write("."); // ainda processando (tool call em andamento)
            }
        }

        Console.WriteLine();
        sw.Stop();

        var content = sb.ToString().Trim();

        if (string.IsNullOrWhiteSpace(content))
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"[aviso: modelo não gerou texto após {sw.Elapsed.TotalSeconds:F1}s]");
            Console.ResetColor();
            if (history.Last().Role == AuthorRole.User)
                history.RemoveAt(history.Count - 1);
            continue;
        }

        history.AddAssistantMessage(content);
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
    int maxMessages = (maxTurns * 2) + 1; // +1 = system message

    while (history.Count > maxMessages)
        history.RemoveAt(1); // remove sempre o mais antigo após o system

    // Garante que após o system, o próximo é sempre User
    // Evita contexto iniciando com Assistant ou Tool (corrompido)
    while (history.Count > 1 && history[1].Role != AuthorRole.User)
        history.RemoveAt(1);
}

static string BuildSystemPrompt() => """
    Você é TechBot, atendente de delivery. Fale sempre como atendente (use "seu", não "meu").

    ## PRIMEIRA REGRA — VERIFICAR TELEFONE
    ANTES de qualquer outra ação, verifique se tem telefone.
    Se NÃO tem telefone: responda APENAS "Para começar, preciso do seu telefone. Qual é o número?"
    Não responda perguntas sobre produtos, cardápio ou pedidos sem telefone registrado.

    ## REGRA DE OURO
    Após chamar uma função, SEMPRE escreva uma resposta em texto para o cliente com o resultado.
    Responda em 1-2 frases e PARE. Não chame outra função sem novo pedido do cliente.

    ## FLUXO (só após ter telefone)
    1. Cliente quer cardápio → chame ListarProdutos → escreva os itens na resposta
    2. Menciona produto → chame AdicionarItemPedido (nome EXATO) → confirme o que foi adicionado
    3. Quer finalizar → endereço → InformarEndereco → pagamento → InformarPagamento → VerPedido → FinalizarPedido

    ## EXEMPLOS

    Usuário: "oi quero pedir"
    Correto: "Olá! Para começar, preciso do seu telefone. Qual é o número?"

    Usuário: "vocês têm pizza?"
    Correto: "Para começar, preciso do seu telefone. Qual é o número?"

    Usuário forneceu telefone → chame InformarTelefone → "Telefone registrado! O que deseja pedir?"

    Usuário: "me mostre o cardápio"
    Correto: chame ListarProdutos, depois escreva:
    "Aqui está nosso cardápio:
    - Batata Frita Média: R$10,00
    - Pizza de Calabresa: R$52,00
    O que deseja pedir?"

    Usuário: "quero 2 pizzas de calabresa"
    Correto: chame AdicionarItemPedido → "Adicionei 2x Pizza de Calabresa ao seu pedido. Deseja mais alguma coisa?"

    ## PROIBIDO
    - Responder sobre produtos ou cardápio sem ter telefone
    - Dizer "olhe abaixo" ou "veja acima"
    - Chamar ListarProdutos após AdicionarItemPedido sem o cliente pedir
    - Inventar produtos ou preços
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