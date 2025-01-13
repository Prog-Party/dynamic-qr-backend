using DynamicQR.Api.Endpoints.QrCodes.QrCodeGetAll;
using FluentAssertions;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using ApplicationResponse = DynamicQR.Application.QrCodes.Queries.GetAllQrCodes.Response;

namespace Api.Tests.Endpoints.QrCodes.QrCodeGetAll;

[ExcludeFromCodeCoverage]
public sealed class QrCodeGetAllMappers
{
    [Fact]
    public void ToContract_QrCodeGet_NullResponse_ReturnsNull()
    {
        // Arrange
        ApplicationResponse? response = null;

        // Act
        var result = Mapper.ToContract(response!);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ToContract_QrCodeGet_ValidResponse_MapsToResponse()
    {
        // Arrange
        var response = new ApplicationResponse
        {
            Id = "123",
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
        result!.Id.Should().Be(response.Id);
        result.BackgroundColor.Should().Be(ColorTranslator.ToHtml(response.BackgroundColor));
        result.ForegroundColor.Should().Be(ColorTranslator.ToHtml(response.ForegroundColor));
        result.ImageHeight.Should().Be(response.ImageHeight.GetValueOrDefault());
        result.ImageUrl.Should().Be(response.ImageUrl);
        result.ImageWidth.Should().Be(response.ImageWidth.GetValueOrDefault());
        result.IncludeMargin.Should().Be(response.IncludeMargin);
    }
}