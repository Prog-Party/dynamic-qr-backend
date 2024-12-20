using Api.Tests.Endpoints.QrCodes.Mocks;
using Api.Tests.Utility;
using DynamicQR.Api.Attributes;
using DynamicQR.Api.Endpoints.QrCodes.QrCodeGetAll;
using DynamicQR.Api.Mappers;
using DynamicQR.Application.QrCodes.Queries.GetAllQrCodes;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Net;
using ApplicationResponse = DynamicQR.Application.QrCodes.Queries.GetAllQrCodes.Response;
using Response = DynamicQR.Api.Endpoints.QrCodes.QrCodeGetAll.Response;

namespace Api.Tests.Endpoints.QrCodes;

[ExcludeFromCodeCoverage]
public class QrCodeGetAllTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ILogger<QrCodeGetAll>> _loggerMock;
    private readonly Mock<ILoggerFactory> _loggerFactoryMock;
    private readonly QrCodeGetAll _endpoint;

    public QrCodeGetAllTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _loggerMock = new Mock<ILogger<QrCodeGetAll>>();

        _loggerMock.Setup(x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()
                    ));

        _loggerFactoryMock = new Mock<ILoggerFactory>();
        _loggerFactoryMock.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(() => _loggerMock.Object);
        _endpoint = new QrCodeGetAll(_mediatorMock.Object, _loggerFactoryMock.Object);
    }

    [Fact(Skip = "Skip this test until middleware is added to the tests")]
    public async Task RunAsync_MissingOrganizationIdHeader_ReturnsBadRequest()
    {
        // Arrange
        var req = HttpRequestDataHelper.CreateWithHeaders(HttpMethod.Get);

        // Act
        var response = await _endpoint.RunAsync(req, It.IsAny<CancellationToken>());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var body = await ((MockHttpResponseData)response).ReadAsStringAsync();
        body.Should().Be(new OpenApiHeaderOrganizationIdentifierAttribute().ErrorMessage);
    }

    [Fact]
    public async Task RunAsync_ReturnsAllQrCodes()
    {
        // Arrange
        var req = HttpRequestDataHelper.CreateWithHeaders(HttpMethod.Get, new Dictionary<string, string>
        {
            { "Organization-Identifier", "org-123" }
        });

        var result = new List<ApplicationResponse>
        {
            new ApplicationResponse {
                Id = "1",
                Value = "Value1",
                BackgroundColor = Color.FromArgb(255, 255, 255),
                ForegroundColor = Color.FromArgb(255, 255, 255)
            },
            new ApplicationResponse {
                Id = "2",
                Value = "Value2",
                BackgroundColor = Color.FromArgb(255, 255, 255),
                ForegroundColor = Color.FromArgb(255, 255, 255)
            }
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<Request>(), default))
            .ReturnsAsync(result);

        // Act
        var response = await _endpoint.RunAsync(req, It.IsAny<CancellationToken>());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await ((MockHttpResponseData)response).ReadAsJsonAsync<List<Response>>();

        TestUtility.TestIfObjectsAreEqual(body, result.Select(x => x.ToContract()!).ToList());
    }

    [Fact]
    public async Task RunAsync_ReturnsEmptyArray_WhenNoQrCodesFound()
    {
        // Arrange
        var req = HttpRequestDataHelper.CreateWithHeaders(HttpMethod.Get, new Dictionary<string, string>
        {
            { "Organization-Identifier", "org-123" }
        });

        var result = new List<ApplicationResponse>
        {
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<Request>(), default))
            .ReturnsAsync(result);

        // Act
        var response = await _endpoint.RunAsync(req, It.IsAny<CancellationToken>());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await ((MockHttpResponseData)response).ReadAsJsonAsync<List<Response>>();

        TestUtility.TestIfObjectsAreEqual(body, result.Select(x => x.ToContract()!).ToList());
    }
}
