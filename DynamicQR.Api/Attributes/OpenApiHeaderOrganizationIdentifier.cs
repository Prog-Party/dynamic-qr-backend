using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;

namespace DynamicQR.Api.Attributes;

public class OpenApiHeaderOrganizationIdentifierAttribute : OpenApiParameterAttribute
{
    public string ErrorMessage
        => $"Missing required header: {Name}";

    public OpenApiHeaderOrganizationIdentifierAttribute() : base("Organization-Identifier")
    {
        Description = "The organization identifier";
        In = ParameterLocation.Header;
        Required = true;
    }
}