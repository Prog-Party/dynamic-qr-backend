using FluentAssertions;
using System.Reflection;

namespace Api.Architecture.Tests;

public sealed class DependencyTests
{
    private readonly Assembly apiAssembly = typeof(DynamicQR.Api.Endpoints.QrCodes.QrCodeGet.QrCodeGet).Assembly;
    private readonly Assembly coreAssembly = typeof(DynamicQR.Domain.Models.QrCode).Assembly;
    private readonly Assembly applicationAssembly = typeof(DynamicQR.Application.QrCodes.Commands.CreateQrCode.CommandHandler).Assembly;
    private readonly Assembly infrastructureAssembly = typeof(DynamicQR.Infrastructure.Services.QrCodeRepositoryService).Assembly;

    [Fact]
    public void ApiAssemblyHasDependenciesNeededToSetupContainer()
    {
        apiAssembly.Should().Reference(applicationAssembly);
        apiAssembly.Should().Reference(infrastructureAssembly);
    }

    [Fact]
    public void CoreAssemblyDoesNotHaveOutwardsDependencies()
    {
        coreAssembly.Should().NotReference(apiAssembly);
        coreAssembly.Should().NotReference(applicationAssembly);
        coreAssembly.Should().NotReference(infrastructureAssembly);
    }

    [Fact]
    public void ApplicationAssemblyHasOnlyInwardsDependencies()
    {
        applicationAssembly.Should().Reference(coreAssembly);
        applicationAssembly.Should().NotReference(apiAssembly);
        applicationAssembly.Should().NotReference(infrastructureAssembly);
    }

    [Fact]
    public void InfrastructureAssemblyHasOnlyInwardsDependencies()
    {
        infrastructureAssembly.Should().Reference(coreAssembly);
        infrastructureAssembly.Should().NotReference(applicationAssembly);
        infrastructureAssembly.Should().NotReference(apiAssembly);
    }
}