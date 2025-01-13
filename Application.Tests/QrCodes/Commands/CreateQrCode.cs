using DynamicQR.Application.QrCodes.Commands.CreateQrCode;
using DynamicQR.Domain.Interfaces;
using DynamicQR.Domain.Models;
using FluentAssertions;
using Moq;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace Application.Tests.QrCodes.Commands;

[ExcludeFromCodeCoverage]
public sealed class CreateQrCode
{
    private readonly Mock<IQrCodeRepositoryService> _qrCodeRepositoryServiceMock;
    private readonly Mock<IQrCodeTargetRepositoryService> _qrCodeTargetRepositoryServiceMock;
    private readonly Mock<IQrCodeHistoryRepositoryService> _qrCodeHistoryRepositoryServiceMock;
    private readonly CommandHandler _handler;

    public CreateQrCode()
    {
        _qrCodeRepositoryServiceMock = new Mock<IQrCodeRepositoryService>();
        _qrCodeTargetRepositoryServiceMock = new Mock<IQrCodeTargetRepositoryService>();
        _qrCodeHistoryRepositoryServiceMock = new Mock<IQrCodeHistoryRepositoryService>();
        _handler = new CommandHandler(_qrCodeRepositoryServiceMock.Object, _qrCodeTargetRepositoryServiceMock.Object, _qrCodeHistoryRepositoryServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesQrCodeAndQrCodeTarget_ReturnsResponse()
    {
        // Arrange
        var command = new Command
        {
            OrganizationId = "org456",
            BackgroundColor = Color.White,
            ForegroundColor = Color.Black,
            ImageHeight = 300,
            ImageWidth = 300,
            ImageUrl = "http://example.com/image.png",
            IncludeMargin = true,
            Value = "TargetValue"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeNullOrEmpty();

        _qrCodeRepositoryServiceMock.Verify(repo =>
            repo.CreateAsync(command.OrganizationId,
                It.Is<QrCode>(qr =>
                    qr.Id == result.Id &&
                    qr.BackgroundColor == command.BackgroundColor &&
                    qr.ForegroundColor == command.ForegroundColor &&
                    qr.ImageHeight == command.ImageHeight &&
                    qr.ImageWidth == command.ImageWidth &&
                    qr.ImageUrl == command.ImageUrl &&
                    qr.IncludeMargin == command.IncludeMargin),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _qrCodeTargetRepositoryServiceMock.Verify(repo =>
            repo.CreateAsync(
                It.Is<QrCodeTarget>(target =>
                    target.QrCodeId == result.Id &&
                    target.Value == command.Value),
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
    public async Task Handle_QrCodeRepositoryThrowsException_PropagatesException()
    {
        // Arrange
        var command = new Command
        {
            OrganizationId = "org456",
            BackgroundColor = Color.White,
            ForegroundColor = Color.Black,
            ImageHeight = 300,
            ImageWidth = 300,
            ImageUrl = "http://example.com/image.png",
            IncludeMargin = true,
            Value = "TargetValue"
        };

        _qrCodeRepositoryServiceMock
            .Setup(repo => repo.CreateAsync(command.OrganizationId, It.IsAny<QrCode>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Repository error"));

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Repository error");

        _qrCodeRepositoryServiceMock.Verify(repo =>
            repo.CreateAsync(command.OrganizationId, It.IsAny<QrCode>(), It.IsAny<CancellationToken>()),
            Times.Once);

        _qrCodeTargetRepositoryServiceMock.Verify(repo =>
            repo.CreateAsync(It.IsAny<QrCodeTarget>(), It.IsAny<CancellationToken>()),
            Times.Never); // Ensure target repository is not called.
    }

    [Fact]
    public async Task Handle_QrCodeTargetRepositoryThrowsException_PropagatesException()
    {
        // Arrange
        var command = new Command
        {
            OrganizationId = "org456",
            BackgroundColor = Color.White,
            ForegroundColor = Color.Black,
            ImageHeight = 300,
            ImageWidth = 300,
            ImageUrl = "http://example.com/image.png",
            IncludeMargin = true,
            Value = "TargetValue"
        };

        _qrCodeTargetRepositoryServiceMock
            .Setup(repo => repo.CreateAsync(It.IsAny<QrCodeTarget>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Target repository error"));

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Target repository error");

        _qrCodeRepositoryServiceMock.Verify(repo =>
            repo.CreateAsync(command.OrganizationId, It.IsAny<QrCode>(), It.IsAny<CancellationToken>()),
            Times.Once);

        _qrCodeTargetRepositoryServiceMock.Verify(repo =>
            repo.CreateAsync(It.IsAny<QrCodeTarget>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public void GenerateQrCodeId_ShouldReturn8CharacterString()
    {
        // Act
        var result = _handler.GenerateQrCodeId();

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Length.Should().Be(8);
        result.Should().MatchRegex("^[a-z0-9]{8}$");
    }
}