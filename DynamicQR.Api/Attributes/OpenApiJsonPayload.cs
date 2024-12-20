using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;

namespace DynamicQR.Api.Attributes;

public class OpenApiJsonPayloadAttribute : OpenApiRequestBodyAttribute
{
    public OpenApiJsonPayloadAttribute(Type type)
        : base("application/json", type)
    {
    }
}