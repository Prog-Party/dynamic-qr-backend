using DynamicQR.Application.QrCodes.Queries.GetQrCode;
using DynamicQR.Domain.Interfaces;
using DynamicQR.Domain.Models;
using FluentAssertions;
using Microsoft.Azure.Storage;
using Moq;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace Application.Tests.QrCodes.Queries;

[ExcludeFromCodeCoverage]
public sealed class GetQrCodeTests
{
    private readonly Mock<IQrCodeRepositoryService> _qrCodeRepositoryServiceMock;
    private readonly Mock<IQrCodeTargetRepositoryService> _qrCodeTargetRepositoryServiceMock;
    private readonly RequestHandler _handler;

    public GetQrCodeTests()
    {
        _qrCodeRepositoryServiceMock = new Mock<IQrCodeRepositoryService>();
        _qrCodeTargetRepositoryServiceMock = new Mock<IQrCodeTargetRepositoryService>();
        _handler = new RequestHandler(_qrCodeRepositoryServiceMock.Object, _qrCodeTargetRepositoryServiceMock.Object);
    }

    [Fact]
    public void Constructor_NullQrCodeRepositoryService_ShouldThrowArgumentNullException()
    {
        // Act
        Action act = () => new RequestHandler(null!, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'qrCodeRepositoryService')");
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldReturnCorrectResponse()
    {
        // Arrange
        var request = new Request
        {
            OrganizationId = "org123",
            Id = "qr123"
        };

        var qrCode = new QrCode
        {
            Id = "qr123",
            BackgroundColor = Color.White,
            ForegroundColor = Color.Black,
            ImageHeight = 200,
            ImageWidth = 200,
            ImageUrl = "http://example.com/qrcode.png",
            IncludeMargin = true
        };

        var qrCodeValue = new QrCodeTarget
        {
            Value = "value"
        };

        _qrCodeRepositoryServiceMock
            .Setup(service => service.ReadAsync(request.OrganizationId, request.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(qrCode);

        _qrCodeTargetRepositoryServiceMock
            .Setup(service => service.ReadAsync(request.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(qrCodeValue);

        // Act
        var response = await _handler.Handle(request, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Value.Should().Be(qrCodeValue.Value);
        response.Id.Should().Be(qrCode.Id);
        response.BackgroundColor.Should().Be(qrCode.BackgroundColor);
        response.ForegroundColor.Should().Be(qrCode.ForegroundColor);
        response.ImageHeight.Should().Be(qrCode.ImageHeight);
        response.ImageWidth.Should().Be(qrCode.ImageWidth);
        response.ImageUrl.Should().Be(qrCode.ImageUrl);
        response.IncludeMargin.Should().Be(qrCode.IncludeMargin);

        _qrCodeRepositoryServiceMock.Verify(service => service.ReadAsync(request.OrganizationId, request.Id, It.IsAny<CancellationToken>()), Times.Once);
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
    public async Task Handle_QrCodeNotFound_ShouldThrowStorageException()
    {
        // Arrange
        var request = new Request
        {
            OrganizationId = "org123",
            Id = "qrNotFound"
        };

        _qrCodeRepositoryServiceMock
            .Setup(service => service.ReadAsync(request.OrganizationId, request.Id, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new StorageException("QrCode not found"));

        // Act
        Func<Task> act = async () => await _handler.Handle(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<StorageException>().WithMessage("QrCode not found");

        _qrCodeRepositoryServiceMock.Verify(service => service.ReadAsync(request.OrganizationId, request.Id, It.IsAny<CancellationToken>()), Times.Once);
    }
}