using DynamicQR.Api.Mappers;
using FluentAssertions;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace Api.Tests.Mappers;

[ExcludeFromCodeCoverage]
public sealed class QrCodesMappersTests
{
    [Fact]
    public void ToCore_QrCodePost_NullRequest_ReturnsNull()
    {
        // Arrange
        DynamicQR.Api.Endpoints.QrCodes.QrCodePost.Request? request = null;
        string organizationId = "org123";

        // Act
        var result = QrCodesMappers.ToCore(request!, organizationId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ToCore_QrCodePost_ValidRequest_MapsToCommand()
    {
        // Arrange
        var request = new DynamicQR.Api.Endpoints.QrCodes.QrCodePost.Request
        {
            BackgroundColor = "#FFFFFF",
            ForegroundColor = "#000000",
            ImageHeight = 100,
            ImageUrl = "https://example.com/image.png",
            ImageWidth = 200,
            IncludeMargin = true,
            Value = "QRCodeValue"
        };

        string organizationId = "org123";

        // Act
        var result = QrCodesMappers.ToCore(request, organizationId);

        // Assert
        result.Should().NotBeNull();
        result!.BackgroundColor.Should().Be(ColorTranslator.FromHtml(request.BackgroundColor));
        result.ForegroundColor.Should().Be(ColorTranslator.FromHtml(request.ForegroundColor));
        result.ImageHeight.Should().Be(request.ImageHeight);
        result.ImageUrl.Should().Be(request.ImageUrl);
        result.ImageWidth.Should().Be(request.ImageWidth);
        result.IncludeMargin.Should().Be(request.IncludeMargin);
        result.Value.Should().Be(request.Value);
        result.OrganisationId.Should().Be(organizationId);
    }

    [Fact]
    public void ToContract_CreateQrCode_NullResponse_ReturnsNull()
    {
        // Arrange
        DynamicQR.Application.QrCodes.Commands.CreateQrCode.Response? response = null;

        // Act
        var result = QrCodesMappers.ToContract(response!);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ToContract_CreateQrCode_ValidResponse_MapsToResponse()
    {
        // Arrange
        var response = new DynamicQR.Application.QrCodes.Commands.CreateQrCode.Response
        {
            Id = "12345"
        };

        // Act
        var result = QrCodesMappers.ToContract(response);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(response.Id);
    }

    [Fact]
    public void ToContract_QrCodeGet_NullResponse_ReturnsNull()
    {
        // Arrange
        DynamicQR.Application.QrCodes.Queries.GetQrCode.Response? response = null;

        // Act
        var result = QrCodesMappers.ToContract(response!);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ToContract_QrCodeGet_ValidResponse_MapsToResponse()
    {
        // Arrange
        var response = new DynamicQR.Application.QrCodes.Queries.GetQrCode.Response
        {
            BackgroundColor = Color.White,
            ForegroundColor = Color.Black,
            ImageHeight = 100,
            ImageUrl = "https://example.com/image.png",
            ImageWidth = 200,
            IncludeMargin = true
        };

        // Act
        var result = QrCodesMappers.ToContract(response);

        // Assert
        result.Should().NotBeNull();
        result!.BackgroundColor.Should().Be(ColorTranslator.ToHtml(response.BackgroundColor));
        result.ForegroundColor.Should().Be(ColorTranslator.ToHtml(response.ForegroundColor));
        result.ImageHeight.Should().Be(response.ImageHeight.GetValueOrDefault());
        result.ImageUrl.Should().Be(response.ImageUrl);
        result.ImageWidth.Should().Be(response.ImageWidth.GetValueOrDefault());
        result.IncludeMargin.Should().Be(response.IncludeMargin);
    }

    [Fact]
    public void ToCore_QrCodePut_NullRequest_ReturnsNull()
    {
        // Arrange
        DynamicQR.Api.Endpoints.QrCodes.QrCodePut.Request? request = null;
        string id = "qr123";
        string organizationId = "org123";

        // Act
        var result = QrCodesMappers.ToCore(request!, id, organizationId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ToCore_QrCodePut_ValidRequest_MapsToCommand()
    {
        // Arrange
        var request = new DynamicQR.Api.Endpoints.QrCodes.QrCodePut.Request
        {
            BackgroundColor = "#FFFFFF",
            ForegroundColor = "#000000",
            ImageHeight = 150,
            ImageUrl = "https://example.com/image_updated.png",
            ImageWidth = 300,
            IncludeMargin = false
        };

        string id = "qr123";
        string organizationId = "org123";

        // Act
        var result = QrCodesMappers.ToCore(request, id, organizationId);

        // Assert
        result.Should().NotBeNull();
        result!.BackgroundColor.Should().Be(ColorTranslator.FromHtml(request.BackgroundColor));
        result.ForegroundColor.Should().Be(ColorTranslator.FromHtml(request.ForegroundColor));
        result.ImageHeight.Should().Be(request.ImageHeight);
        result.ImageUrl.Should().Be(request.ImageUrl);
        result.ImageWidth.Should().Be(request.ImageWidth);
        result.IncludeMargin.Should().Be(request.IncludeMargin);
        result.Id.Should().Be(id);
        result.OrganisationId.Should().Be(organizationId);
    }

    [Fact]
    public void ToContract_QrCodePut_NullResponse_ReturnsNull()
    {
        // Arrange
        DynamicQR.Application.QrCodes.Commands.UpdateQrCode.Response? response = null;

        // Act
        var result = QrCodesMappers.ToContract(response!);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ToContract_QrCodePut_ValidResponse_MapsToResponse()
    {
        // Arrange
        var response = new DynamicQR.Application.QrCodes.Commands.UpdateQrCode.Response
        {
            Id = "qr123"
        };

        // Act
        var result = QrCodesMappers.ToContract(response);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(response.Id);
    }
}