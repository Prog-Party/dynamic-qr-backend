﻿using DynamicQR.Domain.Interfaces;
using DynamicQR.Infrastructure.Services;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace DynamicQR.Infrastructure.Extensions;

[ExcludeFromCodeCoverage]
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">Thrown when the connection string is not found</exception>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        string? connectionString = Environment.GetEnvironmentVariable("QrCodeStorageConnection");

        if (string.IsNullOrEmpty(connectionString))
            throw new InvalidOperationException("No valid connection string found.");

        services.AddTransient<IQrCodeRepositoryService, QrCodeRepositoryService>();
        services.AddTransient<IQrCodeTargetRepositoryService, QrCodeTargetRepositoryService>();
        services.AddTransient<IQrCodeHistoryRepositoryService, QrCodeHistoryRepositoryService>();

        services.AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddTableServiceClient(connectionString);
        });

        return services;
    }
}