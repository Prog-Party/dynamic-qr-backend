using DynamicQR.Api.Endpoints.QrCodes.QrCodePost;
using FluentAssertions;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using ApplicationResponse = DynamicQR.Application.QrCodes.Commands.CreateQrCode.Response;

namespace Api.Tests.Endpoints.QrCodes.QrCodePost;

[ExcludeFromCodeCoverage]
public sealed class QrCodePostMappers
{
    [Fact]
    public void ToCore_QrCodePost_NullRequest_ReturnsNull()
    {
        // Arrange
        QrCodePostRequest? request = null;
        string organizationId = "org123";
        string customerId = "cust123";

        // Act
        var result = Mapper.ToCore(request!, organizationId, customerId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ToCore_QrCodePost_ValidRequest_MapsToCommand()
    {
        // Arrange
        var request = new QrCodePostRequest
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
        string customerId = "cust123";

        // Act
        var result = Mapper.ToCore(request, organizationId, customerId);

        // Assert
        result.Should().NotBeNull();
        result!.BackgroundColor.Should().Be(ColorTranslator.FromHtml(request.BackgroundColor));
        result.ForegroundColor.Should().Be(ColorTranslator.FromHtml(request.ForegroundColor));
        result.ImageHeight.Should().Be(request.ImageHeight);
        result.ImageUrl.Should().Be(request.ImageUrl);
        result.ImageWidth.Should().Be(request.ImageWidth);
        result.IncludeMargin.Should().Be(request.IncludeMargin);
        result.Value.Should().Be(request.Value);
        result.OrganizationId.Should().Be(organizationId);
        result.CustomerId.Should().Be(customerId);
    }

    [Fact]
    public void ToContract_QrCodePost_NullResponse_ReturnsNull()
    {
        // Arrange
        ApplicationResponse? response = null;

        // Act
        var result = Mapper.ToContract(response!);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ToContract_QrCodePost_ValidResponse_MapsToResponse()
    {
        // Arrange
        var response = new ApplicationResponse
        {
            Id = "12345"
        };

        // Act
        var result = Mapper.ToContract(response);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(response.Id);
    }
}