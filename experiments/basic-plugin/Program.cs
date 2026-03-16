using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.Extensions.DependencyInjection;
using Plugins;

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

var kernel = builder.Build();

// registra o plugin
kernel.ImportPluginFromType<LightsPlugin>();

var chat = kernel.GetRequiredService<IChatCompletionService>();

var history = new ChatHistory();

history.AddSystemMessage("""
Você controla as luzes de uma casa.
Use as funções disponíveis quando o usuário perguntar sobre luzes.
""");

history.AddUserMessage("Quais luzes existem?");

var settings = new PromptExecutionSettings()
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
};

var response = await chat.GetChatMessageContentAsync(
    history,
    settings,
    kernel
);

Console.WriteLine(response.Content);