﻿using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;

namespace DynamicQR.Api.Attributes;

public class OpenApiHeaderCustomerIdentifierAttribute : OpenApiParameterAttribute
{
    public string ErrorMessage
        => $"Missing required header: {Name}";

    public OpenApiHeaderCustomerIdentifierAttribute() : base("Customer-Identifier")
    {
        Description = "The customer identifier";
        In = ParameterLocation.Header;
        Required = true;
    }
}