using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using System.Net;

namespace DynamicQR.Api.Attributes;

internal class OpenApiJsonResponseAttribute : OpenApiResponseWithBodyAttribute
{
    public OpenApiJsonResponseAttribute(Type bodyType, HttpStatusCode statusCode = HttpStatusCode.OK)
        : base(statusCode, "application/json; charset=utf-8", bodyType)
    {
    }
}