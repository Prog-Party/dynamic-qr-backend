using DynamicQR.Application.QrCodes.Commands.UpdateQrCodeTarget;
using DynamicQR.Domain.Interfaces;
using DynamicQR.Domain.Models;
using FluentAssertions;
using Moq;
using System.Diagnostics.CodeAnalysis;

namespace Application.Tests.QrCodes.Commands;

[ExcludeFromCodeCoverage]
public sealed class UpdateQrCodeTargetTests
{
    private readonly Mock<IQrCodeTargetRepositoryService> _qrCodeTargetRepositoryServiceMock;
    private readonly CommandHandler _handler;

    public UpdateQrCodeTargetTests()
    {
        _qrCodeTargetRepositoryServiceMock = new Mock<IQrCodeTargetRepositoryService>();
        _handler = new CommandHandler(_qrCodeTargetRepositoryServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_UpdatesQrCodeTarget_ReturnsResponse()
    {
        // Arrange
        var command = new Command
        {
            Id = "123",
            Value = "UpdatedValue"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(command.Id);

        _qrCodeTargetRepositoryServiceMock.Verify(repo =>
            repo.UpdateAsync(It.Is<QrCodeTarget>(target =>
                target.QrCodeId == command.Id &&
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
    public async Task Handle_RepositoryThrowsException_PropagatesException()
    {
        // Arrange
        var command = new Command
        {
            Id = "123",
            Value = "UpdatedValue"
        };

        _qrCodeTargetRepositoryServiceMock
            .Setup(repo => repo.UpdateAsync(It.IsAny<QrCodeTarget>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Repository error"));

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Repository error");

        _qrCodeTargetRepositoryServiceMock.Verify(repo =>
            repo.UpdateAsync(It.IsAny<QrCodeTarget>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_EmptyCommandProperties_StillCallsRepository()
    {
        // Arrange
        var command = new Command
        {
            Id = string.Empty,
            Value = string.Empty
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(command.Id);

        _qrCodeTargetRepositoryServiceMock.Verify(repo =>
            repo.UpdateAsync(It.Is<QrCodeTarget>(target =>
                target.QrCodeId == command.Id &&
                target.Value == command.Value),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_NullValueInCommand_UpdatesQrCodeTargetWithNullValue()
    {
        // Arrange
        var command = new Command
        {
            Id = "123",
            Value = null
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(command.Id);

        _qrCodeTargetRepositoryServiceMock.Verify(repo =>
            repo.UpdateAsync(It.Is<QrCodeTarget>(target =>
                target.QrCodeId == command.Id &&
                target.Value == command.Value),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}