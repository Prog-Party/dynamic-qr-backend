using DynamicQR.Api.Attributes;
using DynamicQR.Api.Extensions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;
using System.Net;

namespace DynamicQR.Api.Middleware;

/// <summary>
/// This middleware validates the HTTP input values and checks if all the neccesary headers/queries have been added.
/// </summary>
public class ValidateHttpRequestMiddleware : IFunctionsWorkerMiddleware
{
    private readonly ILogger<ValidateHttpRequestMiddleware> _logger;

    public ValidateHttpRequestMiddleware(ILogger<ValidateHttpRequestMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        HttpRequestData req = (await context.GetHttpRequestDataAsync())!;

        if (!await ValidateOrganizationIdentifier(context, req))
            return;

        if (!await ValidateCustomerIdentifier(context, req))
            return;

        await next.Invoke(context);
    }

    /// <summary>
    /// Validates the organization identifier in the HTTP request.
    /// When the function has the attribute OpenApiHeader OrganizationIdentifier,
    /// the request must have the header as well
    /// </summary>
    /// <param name="context">The function context.</param>
    /// <param name="req">The HTTP request data.</param>
    /// <returns>True if the organization identifier is valid; otherwise, false.</returns>
    private async Task<bool> ValidateOrganizationIdentifier(FunctionContext context, HttpRequestData req)
    {
        var functionAttributes = context.GetFunctionAttributes();

        if (functionAttributes.Any(x => x is OpenApiHeaderOrganizationIdentifierAttribute)
            && !req.HasHeaderAttribute<OpenApiHeaderOrganizationIdentifierAttribute>())
        {
            var message = new OpenApiHeaderOrganizationIdentifierAttribute().ErrorMessage;
            HttpResponseData errorResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await errorResponse.WriteStringAsync(message);
            _logger.LogError($"Bad Request: {message}");
            return false;
        }

        return true;
    }


    /// <summary>
    /// Validates the organization identifier in the HTTP request.
    /// When the function has the attribute OpenApiHeader OrganizationIdentifier,
    /// the request must have the header as well
    /// </summary>
    /// <param name="context">The function context.</param>
    /// <param name="req">The HTTP request data.</param>
    /// <returns>True if the organization identifier is valid; otherwise, false.</returns>
    private async Task<bool> ValidateCustomerIdentifier(FunctionContext context, HttpRequestData req)
    {
        var functionAttributes = context.GetFunctionAttributes();

        if (functionAttributes.Any(x => x is OpenApiHeaderCustomerIdentifierAttribute)
            && !req.HasHeaderAttribute<OpenApiHeaderCustomerIdentifierAttribute>())
        {
            var message = new OpenApiHeaderCustomerIdentifierAttribute().ErrorMessage;
            HttpResponseData errorResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await errorResponse.WriteStringAsync(message);
            _logger.LogError($"Bad Request: {message}");
            return false;
        }

        return true;
    }
}