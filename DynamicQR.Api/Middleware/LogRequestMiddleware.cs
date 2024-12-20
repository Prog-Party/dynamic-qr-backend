using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;

namespace DynamicQR.Api.Middleware;

/// <summary>
/// Middleware for logging request information.
/// </summary>
public class LogRequestMiddleware : IFunctionsWorkerMiddleware
{
    private readonly ILogger<LogRequestMiddleware> _logger;

    public LogRequestMiddleware(ILogger<LogRequestMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        HttpRequestData req = (await context.GetHttpRequestDataAsync())!;

        _logger.LogInformation($"{context.FunctionDefinition.EntryPoint}.triggered");
        _logger.LogInformation($"Url: {req.Url}");

        await next.Invoke(context);
    }
}
