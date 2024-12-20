using Api.Tests.Endpoints.QrCodes.Mocks;
using Api.Tests.Utility;
using DynamicQR.Api.Attributes;
using DynamicQR.Api.Endpoints.QrCodes.QrCodeGet;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Net;

namespace Api.Tests.Endpoints.QrCodes;

[ExcludeFromCodeCoverage]
public sealed class QrCodeGetTests
{
    private readonly Mock<ILogger<QrCodeGet>> _loggerMock;
    private readonly Mock<ILoggerFactory> _loggerFactoryMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly QrCodeGet _endpoint;

    public QrCodeGetTests()
    {
        _loggerMock = new Mock<ILogger<QrCodeGet>>();
        _loggerMock.Setup(x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()
                ));
        _mediatorMock = new Mock<IMediator>();

        _loggerFactoryMock = new Mock<ILoggerFactory>();
        _loggerFactoryMock.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(() => _loggerMock.Object);

        _endpoint = new QrCodeGet(_mediatorMock.Object, _loggerFactoryMock.Object);
    }

    [Fact(Skip = "Skip this test until middleware is added to the tests")]
    public async Task RunAsync_MissingOrganizationIdHeader_ReturnsBadRequest()
    {
        // Arrange
        var req = HttpRequestDataHelper.CreateWithHeaders(HttpMethod.Get);
        string id = "test-id";

        // Act
        var response = await _endpoint.RunAsync(req, id, It.IsAny<CancellationToken>());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var body = await ((MockHttpResponseData)response).ReadAsStringAsync();
        body.Should().Be(new OpenApiHeaderOrganizationIdentifierAttribute().ErrorMessage);
    }

    [Fact]
    public async Task RunAsync_NoQrCodeFound_ReturnsBadRequest()
    {
        // Arrange
        var req = HttpRequestDataHelper.CreateWithHeaders(HttpMethod.Get, new Dictionary<string, string>
        {
            { "Organization-Identifier", "org-123" }
        });
        string id = "test-id";

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<DynamicQR.Application.QrCodes.Queries.GetQrCode.Request>(), default))
            .ReturnsAsync((DynamicQR.Application.QrCodes.Queries.GetQrCode.Response)null!);

        // Act
        var response = await _endpoint.RunAsync(req, id, It.IsAny<CancellationToken>());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var body = await ((MockHttpResponseData)response).ReadAsStringAsync();
        body.Should().Be("No qr code found with the given identifier.");
    }

    [Fact]
    public async Task RunAsync_ValidQrCodeFound_ReturnsOk()
    {
        // Arrange
        var req = HttpRequestDataHelper.CreateWithHeaders(HttpMethod.Get, new Dictionary<string, string>
        {
            { "Organization-Identifier", "org-123" }
        });
        string id = "test-id";

        var qrCodeResponse = new DynamicQR.Api.Endpoints.QrCodes.QrCodeGet.Response
        {
            IncludeMargin = true,
            BackgroundColor = "#FFFFFF",
            ForegroundColor = "#000000",
            ImageUrl = string.Empty,
            ImageHeight = 256,
            ImageWidth = 256,
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<DynamicQR.Application.QrCodes.Queries.GetQrCode.Request>(), default))
            .ReturnsAsync(new DynamicQR.Application.QrCodes.Queries.GetQrCode.Response
            {
                IncludeMargin = true,
                BackgroundColor = ColorTranslator.FromHtml("#FFFFFF"),
                ForegroundColor = ColorTranslator.FromHtml("#000000"),
                ImageUrl = string.Empty,
                ImageHeight = 256,
                ImageWidth = 256,
            });

        // Act
        var response = await _endpoint.RunAsync(req, id, It.IsAny<CancellationToken>());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await ((MockHttpResponseData)response).ReadAsJsonAsync<Response>();

        TestUtility.TestIfObjectsAreEqual(body, qrCodeResponse);
    }
}