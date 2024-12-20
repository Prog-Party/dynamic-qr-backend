using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;

namespace DynamicQR.Api.Extensions;

internal static class HttpRequestDataExtensions
{
    /// <summary>
    /// Gets the attribute value from the request header based on the specified attribute type.
    /// </summary>
    /// <typeparam name="T">The type of the attribute.</typeparam>
    /// <param name="req">The HTTP request data.</param>
    /// <returns>The attribute value from the request header.</returns>
    internal static string GetHeaderAttribute<T>(this Microsoft.Azure.Functions.Worker.Http.HttpRequestData req)
        where T : OpenApiParameterAttribute
    {
        var parameterIn = Activator.CreateInstance<T>().In;
        if (parameterIn != ParameterLocation.Header)
            throw new ArgumentException("The provided attribute type is not a header attribute.");

        var headerName = Activator.CreateInstance<T>().Name;

        if (req.Headers.TryGetValues(headerName, out var headerValues))
        {
            return headerValues.First();
        }

        return string.Empty;
    }

    internal static bool HasHeaderAttribute<T>(this Microsoft.Azure.Functions.Worker.Http.HttpRequestData req)
        where T : OpenApiParameterAttribute
    {
        var headerName = Activator.CreateInstance<T>().Name;

        return req.Headers.TryGetValues(headerName, out var _);
    }
}
