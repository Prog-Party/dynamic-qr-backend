using Api.Tests.Endpoints.QrCodes.Mocks;
using DynamicQR.Api.Endpoints.QrCodeTargets.QrCodeTargetPut;
using DynamicQR.Api.Mappers;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Api.Tests.Endpoints.QrCodeTargets;

[ExcludeFromCodeCoverage]
public sealed class QrCodeTargetPutTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ILoggerFactory> _loggerFactoryMock;
    private readonly Mock<ILogger<QrCodeTargetPut>> _loggerMock;
    private readonly QrCodeTargetPut _endpoint;

    public QrCodeTargetPutTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _loggerFactoryMock = new Mock<ILoggerFactory>();
        _loggerMock = new Mock<ILogger<QrCodeTargetPut>>();
        _loggerMock.Setup(x => x.Log(
                   LogLevel.Information,
                   It.IsAny<EventId>(),
                   It.IsAny<It.IsAnyType>(),
                   It.IsAny<Exception>(),
                   (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()
               ));

        _loggerFactoryMock.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(() => _loggerMock.Object);

        _endpoint = new QrCodeTargetPut(_mediatorMock.Object, _loggerFactoryMock.Object);
    }

    [Fact]
    public async Task RunAsync_ShouldReturnBadRequest_WhenRequestBodyHasError()
    {
        // Arrange
        var req = HttpRequestDataHelper.CreateWithJsonBody(HttpMethod.Put, new Dictionary<string, string>
        {
            { "Organization-Identifier", "org-123" }
        }, null!);

        var id = "123";

        // Act
        var result = await _endpoint.RunAsync(req, id, It.IsAny<CancellationToken>());

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RunAsync_ShouldReturnBadGateway_WhenStorageExceptionIsThrown()
    {
        Request validRequest = new()
        {
            Value = "new value"
        };

        // Arrange
        var req = HttpRequestDataHelper.CreateWithJsonBody(HttpMethod.Put, new Dictionary<string, string>
        {
            { "Organization-Identifier", "org-123" }
        }, validRequest);

        var id = "123";

        _mediatorMock
            .Setup(mediator => mediator.Send(It.IsAny<DynamicQR.Application.QrCodes.Commands.UpdateQrCodeTarget.Command>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Microsoft.Azure.Storage.StorageException());

        // Act
        var result = await _endpoint.RunAsync(req, id, It.IsAny<CancellationToken>());

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadGateway);
    }

    [Fact]
    public async Task RunAsync_ShouldReturnOk_WhenQrCodeTargetIsUpdatedSuccessfully()
    {
        // Arrange
        var id = "123";

        Request validRequest = new()
        {
            Value = "new value"
        };

        var req = HttpRequestDataHelper.CreateWithJsonBody(HttpMethod.Put, new Dictionary<string, string>
        {
            { "Organization-Identifier", "org-123" }
        }, validRequest);

        var expectedResponse = new DynamicQR.Application.QrCodes.Commands.UpdateQrCodeTarget.Response()
        {
            Id = id,
        };

        _mediatorMock
            .Setup(mediator => mediator.Send(It.IsAny<DynamicQR.Application.QrCodes.Commands.UpdateQrCodeTarget.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        Response? contractResponse = expectedResponse.ToContract();

        // Act
        var result = await _endpoint.RunAsync(req, id, It.IsAny<CancellationToken>());

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await ((MockHttpResponseData)result).ReadAsJsonAsync<Response>();
        body.Should().NotBeNull();
        body!.Id.Should().Be("123");
    }
}