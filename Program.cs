using Microsoft.SemanticKernel;

var builder = Kernel.CreateBuilder();

builder.AddOllamaChatCompletion(
    modelId: "llama3",
    endpoint: new Uri("http://localhost:11434")
);



var kernel = builder.Build();

var result = await kernel.InvokePromptAsync("Explain Semantic Kernel in one sentence");



Console.WriteLine(result);