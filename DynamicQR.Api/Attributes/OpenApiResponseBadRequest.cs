using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using System.Net;

namespace DynamicQR.Api.Attributes;

internal class OpenApiResponseBadRequestAttribute : OpenApiResponseWithoutBodyAttribute
{
    public OpenApiResponseBadRequestAttribute() : base(HttpStatusCode.BadRequest)
    {
        Description = "The payload could not be parsed";
    }
}