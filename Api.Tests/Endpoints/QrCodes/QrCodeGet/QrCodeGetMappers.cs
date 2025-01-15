using DynamicQR.Api.Endpoints.QrCodes.QrCodeGet;
using FluentAssertions;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using ApplicationResponse = DynamicQR.Application.QrCodes.Queries.GetQrCode.Response;

namespace Api.Tests.Endpoints.QrCodes.QrCodeGetMappersTests;

[ExcludeFromCodeCoverage]
public sealed class QrCodeGetMappers
{
    [Fact]
    public void ToContract_QrCodeGet_NullResponse_ReturnsNull()
    {
        // Arrange
        ApplicationResponse? response = null;

        // Act
        Assert.Throws<ArgumentNullException>(() => Mapper.ToContract(response));
    }

    [Fact]
    public void ToContract_QrCodeGet_ValidResponse_MapsToResponse()
    {
        // Arrange
        var response = new ApplicationResponse
        {
            BackgroundColor = Color.White,
            ForegroundColor = Color.Black,
            ImageHeight = 100,
            ImageUrl = "https://example.com/image.png",
            ImageWidth = 200,
            IncludeMargin = true
        };

        // Act
        var result = Mapper.ToContract(response);

        // Assert
        result.Should().NotBeNull();
        result!.BackgroundColor.Should().Be(ColorTranslator.ToHtml(response.BackgroundColor));
        result.ForegroundColor.Should().Be(ColorTranslator.ToHtml(response.ForegroundColor));
        result.ImageHeight.Should().Be(response.ImageHeight.GetValueOrDefault());
        result.ImageUrl.Should().Be(response.ImageUrl);
        result.ImageWidth.Should().Be(response.ImageWidth.GetValueOrDefault());
        result.IncludeMargin.Should().Be(response.IncludeMargin);
    }
}