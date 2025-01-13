using Application.Tests.Utility;
using DynamicQR.Application.QrCodes.Queries.GetQrCodeHistory;
using DynamicQR.Domain.Interfaces;
using DynamicQR.Domain.Models;
using FluentAssertions;
using Moq;
using System.Diagnostics.CodeAnalysis;

namespace Application.Tests.QrCodes.Queries;

[ExcludeFromCodeCoverage]
public sealed class GetQrCodeHistory
{
    private readonly Mock<IQrCodeHistoryRepositoryService> _qrCodeHistoryRepositoryServiceMock;
    private readonly RequestHandler _handler;

    public GetQrCodeHistory()
    {
        _qrCodeHistoryRepositoryServiceMock = new Mock<IQrCodeHistoryRepositoryService>();
        _handler = new RequestHandler(_qrCodeHistoryRepositoryServiceMock.Object);
    }

    [Fact]
    public void Constructor_NullQrCodeHistoryRepositoryService_ShouldThrowArgumentNullException()
    {
        // Act
        Action act = () => new RequestHandler(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'qrCodeHistoryRepositoryService')");
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldReturnCorrectResponse()
    {
        // Arrange
        var request = new Request
        {
            OrganizationId = "org123",
            QrCodeId = "qr123"
        };

        var result = new List<QrCodeHistory> {
            new QrCodeHistory
            {
                QrCodeId = request.QrCodeId,
                Order = "abc",
                OrganizationId = request.OrganizationId,
                CustomerId = "cust123",
                Timestamp = DateTime.UtcNow,
                EventType = "EventType",
                Details = new Dictionary<string, string>
                {
                    { "Key1", "Value1" },
                    { "Key2", "Value2" }
                }
            }
        };

        _qrCodeHistoryRepositoryServiceMock
            .Setup(service => service.GetHistoryAsync(request.QrCodeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var response = await _handler.Handle(request, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();

        TestUtility.TestIfObjectsAreEqual(response, result);

        _qrCodeHistoryRepositoryServiceMock.Verify(service => service.GetHistoryAsync(request.QrCodeId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NullRequest_ShouldThrowArgumentNullException()
    {
        // Act
        Func<Task> act = async () => await _handler.Handle(null!, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'request')");
    }
}