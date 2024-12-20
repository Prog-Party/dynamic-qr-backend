﻿using Api.Tests.Endpoints.QrCodes.Mocks;
using DynamicQR.Api.Attributes;
using DynamicQR.Api.Endpoints.QrCodes.QrCodeDelete;
using DynamicQR.Domain.Exceptions;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Api.Tests.Endpoints.QrCodes;

[ExcludeFromCodeCoverage]
public sealed class QrCodeDeleteTests
{
    private readonly Mock<ILogger<QrCodeDelete>> _loggerMock;
    private readonly Mock<ILoggerFactory> _loggerFactoryMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly QrCodeDelete _endpoint;

    public QrCodeDeleteTests()
    {
        _loggerMock = new Mock<ILogger<QrCodeDelete>>();
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

        _endpoint = new QrCodeDelete(_mediatorMock.Object, _loggerFactoryMock.Object);
    }

    [Fact(Skip = "Skip this test until middleware is added to the tests")]
    public async Task RunAsync_MissingOrganizationHeader_ReturnsBadRequest()
    {
        // Arrange
        var req = HttpRequestDataHelper.CreateWithHeaders(HttpMethod.Delete);
        string id = "qr123";

        // Act
        var response = await _endpoint.RunAsync(req, id, It.IsAny<CancellationToken>());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var body = await ((MockHttpResponseData)response).ReadAsStringAsync();
        body.Should().Be(new OpenApiHeaderOrganizationIdentifierAttribute().ErrorMessage);
    }

    [Fact]
    public async Task RunAsync_ValidRequest_ReturnsNoContent()
    {
        // Arrange
        var req = HttpRequestDataHelper.CreateWithHeaders(HttpMethod.Delete, new Dictionary<string, string>
        {
            { "Organization-Identifier", "org-123" }
        });

        string id = "qr123";

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<DynamicQR.Application.QrCodes.Commands.DeleteQrCode.Command>(), It.IsAny<CancellationToken>()))
            .Returns(Unit.Task);

        // Act
        var result = await _endpoint.RunAsync(req, id, It.IsAny<CancellationToken>());

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);

        _mediatorMock.Verify(m => m.Send(It.Is<DynamicQR.Application.QrCodes.Commands.DeleteQrCode.Command>(cmd =>
            cmd.Id == id && cmd.OrganisationId == "org-123"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RunAsync_StorageException_ReturnsBadGateway()
    {
        // Arrange
        var req = HttpRequestDataHelper.CreateWithHeaders(HttpMethod.Delete, new Dictionary<string, string>
        {
            { "Organization-Identifier", "org-123" }
        });
        string id = "qr123";

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<DynamicQR.Application.QrCodes.Commands.DeleteQrCode.Command>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Microsoft.Azure.Storage.StorageException());

        // Act
        var result = await _endpoint.RunAsync(req, id, It.IsAny<CancellationToken>());

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadGateway);
    }

    [Fact]
    public async Task RunAsync_QrCodeNotFoundException_ReturnsNotFound()
    {
        // Arrange
        var req = HttpRequestDataHelper.CreateWithHeaders(HttpMethod.Delete, new Dictionary<string, string>
        {
            { "Organization-Identifier", "org-123" }
        });
        string id = "qr123";

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<DynamicQR.Application.QrCodes.Commands.DeleteQrCode.Command>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new QrCodeNotFoundException("org-123", id, null));

        // Act
        var result = await _endpoint.RunAsync(req, id, It.IsAny<CancellationToken>());

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact(Skip = "Skip this test until middleware is added to the tests")]
    public async Task RunAsync_InvalidRequest_ReturnsBadRequest()
    {
        // Arrange
        var req = HttpRequestDataHelper.CreateWithHeaders(HttpMethod.Delete, new Dictionary<string, string>
        {
            { "Wrong-Header", "org-123" }
        });
        string id = "qr123";

        // Act
        var response = await _endpoint.RunAsync(req, id, It.IsAny<CancellationToken>());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var body = await ((MockHttpResponseData)response).ReadAsStringAsync();
        body.Should().Be(new OpenApiHeaderOrganizationIdentifierAttribute().ErrorMessage);
    }
}