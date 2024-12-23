using Api.Tests.Endpoints.Mocks;
using Api.Tests.Utility;
using DynamicQR.Api.Attributes;
using DynamicQR.Api.Endpoints.QrCodes.HistoryGet;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using ApplicationRequest = DynamicQR.Application.QrCodes.Queries.GetQrCodeHistory.Request;
using ApplicationResponse = DynamicQR.Application.QrCodes.Queries.GetQrCodeHistory.Response;
using HistoryGetEndpoint = DynamicQR.Api.Endpoints.QrCodes.HistoryGet.HistoryGet;
using Response = DynamicQR.Api.Endpoints.QrCodes.HistoryGet.Response;

namespace Api.Tests.Endpoints.QrCodes.HistoryGet;

[ExcludeFromCodeCoverage]
public sealed class HistoryGetTests
{
    private readonly Mock<ILogger<HistoryGetEndpoint>> _loggerMock;
    private readonly Mock<ILoggerFactory> _loggerFactoryMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly HistoryGetEndpoint _endpoint;

    public HistoryGetTests()
    {
        _loggerMock = new Mock<ILogger<HistoryGetEndpoint>>();
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

        _endpoint = new HistoryGetEndpoint(_mediatorMock.Object, _loggerFactoryMock.Object);
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
    public async Task RunAsync_NoQrCodeFound_ReturnsEmptyList()
    {
        // Arrange
        var req = HttpRequestDataHelper.CreateWithHeaders(HttpMethod.Get, new Dictionary<string, string>
        {
            { "Organization-Identifier", "org-123" }
        });
        string qrCodeId = "test-id";

        var result = new List<ApplicationResponse>
        {
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<ApplicationRequest>(), default))
            .ReturnsAsync(result);

        // Act
        var response = await _endpoint.RunAsync(req, qrCodeId, It.IsAny<CancellationToken>());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await ((MockHttpResponseData)response).ReadAsJsonAsync<List<Response>>();

        TestUtility.TestIfObjectsAreEqual(body, result.Select(Mapper.ToContract).Select(x => x!).ToList());
    }

    [Fact]
    public async Task RunAsync_ReturnList_ReturnsOk()
    {
        // Arrange
        var req = HttpRequestDataHelper.CreateWithHeaders(HttpMethod.Get, new Dictionary<string, string>
        {
            { "Organization-Identifier", "org-123" }
        });
        string qrCodeId = "test-id";

        var result = new List<ApplicationResponse>
        {
            new ApplicationResponse {
                QrCodeId = qrCodeId,
                CustomerId = "1A",
                OrganizationId = "org-123",
                EventType = QrCodeEvents.Lifecycle.Created,
                Order = "20241221-1"
            },
            new ApplicationResponse {
                QrCodeId = qrCodeId,
                CustomerId = "1B",
                OrganizationId = "org-123",
                EventType = QrCodeEvents.Lifecycle.Updated,
                Order = "20241221-2"
            }
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<ApplicationRequest>(), default))
            .ReturnsAsync(result);

        // Act
        var response = await _endpoint.RunAsync(req, qrCodeId, It.IsAny<CancellationToken>());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await ((MockHttpResponseData)response).ReadAsJsonAsync<List<Response>>();

        TestUtility.TestIfObjectsAreEqual(body, result.Select(Mapper.ToContract).Select(x => x!).ToList());
    }
}
