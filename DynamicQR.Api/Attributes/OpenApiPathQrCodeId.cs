using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;

namespace DynamicQR.Api.Attributes;

public class OpenApiPathIdentifier : OpenApiParameterAttribute
{
    public OpenApiPathIdentifier() : base("id")
    {
        Description = "Identifier";
        In = ParameterLocation.Path;
        Required = true;
    }
}