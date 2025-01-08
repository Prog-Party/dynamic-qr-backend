using DynamicQR.Application.QrCodes.Commands.UpdateQrCode;
using DynamicQR.Domain.Interfaces;
using DynamicQR.Domain.Models;
using FluentAssertions;
using Moq;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace Application.Tests.QrCodes.Commands;

[ExcludeFromCodeCoverage]
public sealed class UpdateQrCodeTests
{
    private readonly Mock<IQrCodeRepositoryService> _qrCodeRepositoryServiceMock;
    private readonly Mock<IQrCodeHistoryRepositoryService> _qrCodeHistoryRepositoryServiceMock;
    private readonly CommandHandler _handler;

    public UpdateQrCodeTests()
    {
        _qrCodeRepositoryServiceMock = new Mock<IQrCodeRepositoryService>();
        _qrCodeHistoryRepositoryServiceMock = new Mock<IQrCodeHistoryRepositoryService>();
        _handler = new CommandHandler(_qrCodeRepositoryServiceMock.Object, _qrCodeHistoryRepositoryServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_UpdatesQrCodeAndReturnsResponse()
    {
        // Arrange
        var command = new Command
        {
            Id = "qr123",
            OrganizationId = "org456",
            BackgroundColor = Color.White,
            ForegroundColor = Color.Black,
            ImageHeight = 300,
            ImageWidth = 300,
            ImageUrl = "http://example.com/image.png",
            IncludeMargin = true
        };

        _qrCodeRepositoryServiceMock
            .Setup(repo => repo.UpdateAsync(command.OrganizationId, It.IsAny<QrCode>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(command.Id);

        _qrCodeRepositoryServiceMock.Verify(repo =>
            repo.UpdateAsync(command.OrganizationId,
                It.Is<QrCode>(qr =>
                    qr.Id == command.Id &&
                    qr.BackgroundColor == command.BackgroundColor &&
                    qr.ForegroundColor == command.ForegroundColor &&
                    qr.ImageHeight == command.ImageHeight &&
                    qr.ImageWidth == command.ImageWidth &&
                    qr.ImageUrl == command.ImageUrl &&
                    qr.IncludeMargin == command.IncludeMargin),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_NullCommand_ThrowsArgumentNullException()
    {
        // Arrange
        Command? command = null;

        // Act
        Func<Task> act = async () => await _handler.Handle(command!, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task Handle_RepositoryThrowsException_PropagatesException()
    {
        // Arrange
        var command = new Command
        {
            Id = "qr123",
            OrganizationId = "org456",
            BackgroundColor = Color.White,
            ForegroundColor = Color.Black,
            ImageHeight = 300,
            ImageWidth = 300,
            ImageUrl = "http://example.com/image.png",
            IncludeMargin = true
        };

        _qrCodeRepositoryServiceMock
            .Setup(repo => repo.UpdateAsync(command.OrganizationId, It.IsAny<QrCode>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Repository exception"));

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Repository exception");

        _qrCodeRepositoryServiceMock.Verify(repo =>
            repo.UpdateAsync(command.OrganizationId, It.IsAny<QrCode>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}