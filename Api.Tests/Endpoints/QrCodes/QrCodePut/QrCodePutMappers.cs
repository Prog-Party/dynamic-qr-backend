using DynamicQR.Api.Endpoints.QrCodes.QrCodePut;
using FluentAssertions;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using ApplicationResponse = DynamicQR.Application.QrCodes.Commands.UpdateQrCode.Response;

namespace Api.Tests.Endpoints.QrCodes;

[ExcludeFromCodeCoverage]
public sealed class QrCodePutMappers
{
    [Fact]
    public void ToCore_QrCodePut_NullRequest_ReturnsNull()
    {
        // Arrange
        QrCodePutRequest? request = null;
        string id = "qr123";
        string organizationId = "org123";
        string customerId = "cust123";

        // Act
        var result = Mapper.ToCore(request!, id, organizationId, customerId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ToCore_QrCodePut_ValidRequest_MapsToCommand()
    {
        // Arrange
        var request = new QrCodePutRequest
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
        string customerId = "cust123";

        // Act
        var result = Mapper.ToCore(request, id, organizationId, customerId);

        // Assert
        result.Should().NotBeNull();
        result!.BackgroundColor.Should().Be(ColorTranslator.FromHtml(request.BackgroundColor));
        result.ForegroundColor.Should().Be(ColorTranslator.FromHtml(request.ForegroundColor));
        result.ImageHeight.Should().Be(request.ImageHeight);
        result.ImageUrl.Should().Be(request.ImageUrl);
        result.ImageWidth.Should().Be(request.ImageWidth);
        result.IncludeMargin.Should().Be(request.IncludeMargin);
        result.Id.Should().Be(id);
        result.OrganizationId.Should().Be(organizationId);
        result.CustomerId.Should().Be(customerId);
    }

    [Fact]
    public void ToContract_QrCodePut_NullResponse_ReturnsNull()
    {
        // Arrange
        ApplicationResponse? response = null;

        // Act
        var result = Mapper.ToContract(response!);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ToContract_QrCodePut_ValidResponse_MapsToResponse()
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