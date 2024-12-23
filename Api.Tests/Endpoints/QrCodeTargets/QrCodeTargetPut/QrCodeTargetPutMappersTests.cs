﻿using DynamicQR.Api.Endpoints.QrCodeTargets.QrCodeTargetPut;
using FluentAssertions;
using System.Diagnostics.CodeAnalysis;
using ApplicationResponse = DynamicQR.Application.QrCodes.Commands.UpdateQrCodeTarget.Response;

namespace Api.Tests.Endpoints.QrCodeTargets.QrCodeTargetPut;

[ExcludeFromCodeCoverage]
public sealed class QrCodeTargetPutMappersTests
{
    [Fact]
    public void ToCore_QrCodeTarget_NullRequest_ReturnsNull()
    {
        // Arrange
        Request? request = null;
        string id = "qr123";

        // Act
        var result = Mapper.ToCore(request!, id);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ToCore_QrCodeTarget_ValidRequest_MapsToCommand()
    {
        // Arrange
        var request = new Request
        {
            Value = "NewValue"
        };
        string id = "qr123";

        // Act
        var result = Mapper.ToCore(request, id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(id);
        result.Value.Should().Be(request.Value);
    }

    [Fact]
    public void ToContract_QrCodeTarget_NullResponse_ReturnsNull()
    {
        // Arrange
        ApplicationResponse? response = null;

        // Act
        var result = Mapper.ToContract(response!);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ToContract_QrCodeTarget_ValidResponse_MapsToResponse()
    {
        // Arrange
        var response = new ApplicationResponse
        {
            Id = "qr123"
        };

        // Act
        var result = Mapper.ToContract(response);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(response.Id);
    }
}