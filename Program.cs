using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;
using Microsoft.Extensions.DependencyInjection;

var builder = Kernel.CreateBuilder();

builder.AddOllamaChatCompletion(
    modelId: "phi3",
    endpoint: new Uri("http://localhost:11434")
);

// aumenta timeout
builder.Services.AddHttpClient("ollama", client =>
{
    client.Timeout = TimeSpan.FromMinutes(10);
});

var kernel = builder.Build();

kernel.ImportPluginFromType<LightsPlugin>();

var chat = kernel.GetRequiredService<IChatCompletionService>();

var history = new ChatHistory();
history.AddUserMessage("Quais luzes existem?");

var response = await chat.GetChatMessageContentAsync(
    history,
    kernel: kernel
);

Console.WriteLine(response.Content);