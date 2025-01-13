using DynamicQR.Application.QrCodes.Queries.GetQrCodeTarget;
using DynamicQR.Domain.Interfaces;
using DynamicQR.Domain.Models;
using FluentAssertions;
using Microsoft.Azure.Storage;
using Moq;
using System.Diagnostics.CodeAnalysis;

namespace Application.Tests.QrCodes.Queries;

[ExcludeFromCodeCoverage]
public sealed class GetQrCodeTarget
{
    private readonly Mock<IQrCodeTargetRepositoryService> _qrCodeTargetRepositoryServiceMock;
    private readonly RequestHandler _handler;

    public GetQrCodeTarget()
    {
        _qrCodeTargetRepositoryServiceMock = new Mock<IQrCodeTargetRepositoryService>();
        _handler = new RequestHandler(_qrCodeTargetRepositoryServiceMock.Object);
    }

    [Fact]
    public void Constructor_NullQrCodeTargetRepositoryService_ShouldThrowArgumentNullException()
    {
        // Act
        Action act = () => new RequestHandler(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'qrCodeRepositoryService')");
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldReturnCorrectResponse()
    {
        // Arrange
        var request = new Request
        {
            Id = "qr123"
        };

        var qrCodeTarget = new QrCodeTarget
        {
            QrCodeId = "qr123",
            Value = "SampleValue"
        };

        _qrCodeTargetRepositoryServiceMock
            .Setup(service => service.ReadAsync(request.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(qrCodeTarget);

        // Act
        var response = await _handler.Handle(request, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.QrCodeId.Should().Be(qrCodeTarget.QrCodeId);
        response.Value.Should().Be(qrCodeTarget.Value);

        _qrCodeTargetRepositoryServiceMock.Verify(service => service.ReadAsync(request.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NullRequest_ShouldThrowArgumentNullException()
    {
        // Act
        Func<Task> act = async () => await _handler.Handle(null!, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'request')");
    }

    [Fact]
    public async Task Handle_QrCodeTargetNotFound_ShouldThrowStorageException()
    {
        // Arrange
        var request = new Request
        {
            Id = "nonexistentId"
        };

        _qrCodeTargetRepositoryServiceMock
            .Setup(service => service.ReadAsync(request.Id, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new StorageException("QrCodeTarget not found"));

        // Act
        Func<Task> act = async () => await _handler.Handle(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<StorageException>().WithMessage("QrCodeTarget not found");

        _qrCodeTargetRepositoryServiceMock.Verify(service => service.ReadAsync(request.Id, It.IsAny<CancellationToken>()), Times.Once);
    }
}