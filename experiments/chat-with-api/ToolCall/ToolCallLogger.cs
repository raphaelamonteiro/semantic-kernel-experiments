using Microsoft.SemanticKernel;

namespace chat_with_api.ToolCall;

public class ToolCallLogger : IFunctionInvocationFilter
{
    public async Task OnFunctionInvocationAsync(
        FunctionInvocationContext context,
        Func<FunctionInvocationContext, Task> next)
    {
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine($"[TOOL] {context.Function.Name}({FormatArgs(context.Arguments)})");
        Console.ResetColor();

        await next(context);

        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine($"[TOOL resultado] {context.Result}");
        Console.ResetColor();
    }

    private static string FormatArgs(KernelArguments args)
    {
        if (!args.Any()) return "";
        return string.Join(", ", args.Select(a => $"{a.Key}={a.Value}"));
    }
}