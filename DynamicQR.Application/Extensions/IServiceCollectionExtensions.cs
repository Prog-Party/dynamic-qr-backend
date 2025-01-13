using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace DynamicQR.Application.Extensions;

[ExcludeFromCodeCoverage]
public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        Assembly[] assemblies =
        [
            Assembly.GetAssembly(typeof(QrCodes.Commands.CreateQrCode.CommandHandler))!,
            Assembly.GetAssembly(typeof(QrCodes.Commands.UpdateQrCodeTarget.CommandHandler))!,
            Assembly.GetAssembly(typeof(QrCodes.Queries.GetQrCode.RequestHandler))!,
            Assembly.GetAssembly(typeof(QrCodes.Queries.GetQrCodeTarget.RequestHandler))!,
            Assembly.GetAssembly(typeof(QrCodes.Queries.GetQrCodeHistory.RequestHandler))!
        ];

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assemblies));

        return services;
    }
}