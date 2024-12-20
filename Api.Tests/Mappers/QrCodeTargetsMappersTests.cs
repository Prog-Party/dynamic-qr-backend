using DynamicQR.Api.Mappers;
using FluentAssertions;
using System.Diagnostics.CodeAnalysis;

namespace Api.Tests.Mappers;

[ExcludeFromCodeCoverage]
public sealed class QrCodeTargetsMappersTests
{
    [Fact]
    public void ToCore_NullRequest_ReturnsNull()
    {
        // Arrange
        DynamicQR.Api.Endpoints.QrCodeTargets.QrCodeTargetPut.Request? request = null;
        string id = "qr123";

        // Act
        var result = QrCodeTargetsMappers.ToCore(request!, id);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ToCore_ValidRequest_MapsToCommand()
    {
        // Arrange
        var request = new DynamicQR.Api.Endpoints.QrCodeTargets.QrCodeTargetPut.Request
        {
            Value = "NewValue"
        };
        string id = "qr123";

        // Act
        var result = QrCodeTargetsMappers.ToCore(request, id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(id);
        result.Value.Should().Be(request.Value);
    }

    [Fact]
    public void ToContract_NullResponse_ReturnsNull()
    {
        // Arrange
        DynamicQR.Application.QrCodes.Commands.UpdateQrCodeTarget.Response? response = null;

        // Act
        var result = QrCodeTargetsMappers.ToContract(response!);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ToContract_ValidResponse_MapsToResponse()
    {
        // Arrange
        var response = new DynamicQR.Application.QrCodes.Commands.UpdateQrCodeTarget.Response
        {
            Id = "qr123"
        };

        // Act
        var result = QrCodeTargetsMappers.ToContract(response);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(response.Id);
    }
}