using Api.Tests.Endpoints.QrCodes.Mocks;
using DynamicQR.Api.Attributes;
using DynamicQR.Api.Endpoints;
using DynamicQR.Api.Endpoints.QrCodes.QrCodePost;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Api.Tests.Endpoints.QrCodes;

[ExcludeFromCodeCoverage]
public sealed class QrCodePostTest
{
    private readonly Mock<ILogger<QrCodePost>> _loggerMock;
    private readonly Mock<ILoggerFactory> _loggerFactoryMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly QrCodePost _endpoint;

    public QrCodePostTest()
    {
        _loggerMock = new Mock<ILogger<QrCodePost>>();
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

        _endpoint = new QrCodePost(_mediatorMock.Object, _loggerFactoryMock.Object);
    }

    [Fact(Skip = "Skip this test until middleware is added to the tests")]
    public async Task RunAsync_MissingOrganizationHeader_ReturnsBadRequest()
    {
        // Arrange
        var req = HttpRequestDataHelper.CreateWithHeaders(HttpMethod.Post, []);

        // Act
        var result = await _endpoint.RunAsync(req, It.IsAny<CancellationToken>());

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var body = await ((MockHttpResponseData)result).ReadAsStringAsync();
        body.Should().Be(new OpenApiHeaderOrganizationIdentifierAttribute().ErrorMessage);
    }

    [Fact]
    public async Task RunAsync_InvalidRequestBody_ReturnsBadRequest()
    {
        // Arrange
        var req = HttpRequestDataHelper.CreateWithJsonBody(HttpMethod.Post, new Dictionary<string, string>
        {
            { "Organization-Identifier", "org-123" }
        }, null!);

        // Act
        var result = await _endpoint.RunAsync(req, It.IsAny<CancellationToken>());

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var body = await ((MockHttpResponseData)result).ReadAsStringAsync();
        body.Should().Be(EndpointsBase.ParseBodySerializationError);
    }

    [Fact]
    public async Task RunAsync_ValidRequest_ReturnsCreatedResponse()
    {
        // Arrange
        var validRequest = new Request
        {
            BackgroundColor = "#FFFFFF",
            ForegroundColor = "#000000",
            ImageHeight = 200,
            ImageWidth = 200,
            Value = "TestQRCode",
            IncludeMargin = true,
            ImageUrl = "https://example.com/test.png"
        };

        var expectedCoreResponse = new DynamicQR.Application.QrCodes.Commands.CreateQrCode.Response
        {
            Id = "qr123"
        };

        var req = HttpRequestDataHelper.CreateWithJsonBody(HttpMethod.Post, new Dictionary<string, string>
        {
            { "Organization-Identifier", "org-123" }
        }, validRequest);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<DynamicQR.Application.QrCodes.Commands.CreateQrCode.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedCoreResponse);

        // Act
        var result = await _endpoint.RunAsync(req, It.IsAny<CancellationToken>());

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Created);

        var responseBody = await ((MockHttpResponseData)result).ReadAsJsonAsync<Response>();
        responseBody!.Id.Should().Be(expectedCoreResponse.Id);

        _mediatorMock.Verify(m => m.Send(It.IsAny<DynamicQR.Application.QrCodes.Commands.CreateQrCode.Command>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RunAsync_StorageException_ReturnsBadGateway()
    {
        // Arrange
        var validRequest = new Request
        {
            BackgroundColor = "#FFFFFF",
            ForegroundColor = "#000000",
            ImageHeight = 200,
            ImageWidth = 200,
            Value = "TestQRCode",
            IncludeMargin = true,
            ImageUrl = "https://example.com/test.png"
        };

        var req = HttpRequestDataHelper.CreateWithJsonBody(HttpMethod.Post, new Dictionary<string, string>
        {
            { "Organization-Identifier", "org-123" }
        }, validRequest);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<DynamicQR.Application.QrCodes.Commands.CreateQrCode.Command>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Microsoft.Azure.Storage.StorageException());

        // Act
        var result = await _endpoint.RunAsync(req, It.IsAny<CancellationToken>());

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadGateway);
    }

    [Fact(Skip = "Skip this test until middleware is added to the tests")]
    public async Task RunAsync_EmptyRequestBody_ReturnsBadRequest()
    {
        // Arrange
        var req = HttpRequestDataHelper.CreateWithJsonBody(HttpMethod.Post, new Dictionary<string, string> { { "Organization-Identifier", "org123" } }, string.Empty);

        // Act
        var result = await _endpoint.RunAsync(req, It.IsAny<CancellationToken>());

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var body = await ((MockHttpResponseData)result).ReadAsStringAsync();
        body.Should().Be(new OpenApiHeaderOrganizationIdentifierAttribute().ErrorMessage);
    }

    [Fact(Skip = "Skip this test until middleware is added to the tests")]
    public async Task RunAsync_IncorrectHeaderName_ReturnsBadRequest()
    {
        // Arrange
        var validRequest = new Request
        {
            BackgroundColor = "#FFFFFF",
            ForegroundColor = "#000000",
            ImageHeight = 200,
            ImageWidth = 200,
            IncludeMargin = true,
            ImageUrl = "https://example.com/updated.png"
        };

        var req = HttpRequestDataHelper.CreateWithJsonBody(HttpMethod.Post, new Dictionary<string, string> { { "Wrong-Header", "org123" } }, validRequest);

        // Act
        var result = await _endpoint.RunAsync(req, It.IsAny<CancellationToken>());

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var body = await ((MockHttpResponseData)result).ReadAsStringAsync();
        body.Should().Be(new OpenApiHeaderOrganizationIdentifierAttribute().ErrorMessage);
    }
}