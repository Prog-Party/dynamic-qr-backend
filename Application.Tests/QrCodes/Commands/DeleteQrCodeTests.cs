﻿using DynamicQR.Application.QrCodes.Commands.DeleteQrCode;
using DynamicQR.Domain.Interfaces;
using FluentAssertions;
using MediatR;
using Moq;
using System.Diagnostics.CodeAnalysis;

namespace Application.Tests.QrCodes.Commands;

[ExcludeFromCodeCoverage]
public sealed class DeleteQrCodeTests
{
    private readonly Mock<IQrCodeRepositoryService> _qrCodeRepositoryServiceMock;
    private readonly Mock<IQrCodeTargetRepositoryService> _qrCodeTargetRepositoryServiceMock;
    private readonly CommandHandler _handler;

    public DeleteQrCodeTests()
    {
        _qrCodeRepositoryServiceMock = new Mock<IQrCodeRepositoryService>();
        _qrCodeTargetRepositoryServiceMock = new Mock<IQrCodeTargetRepositoryService>();
        _handler = new CommandHandler(_qrCodeRepositoryServiceMock.Object, _qrCodeTargetRepositoryServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_CallsRepositoryAndReturnsUnit()
    {
        // Arrange
        var command = new Command
        {
            Id = "qr123",
            OrganisationId = "org456"
        };

        _qrCodeRepositoryServiceMock
            .Setup(repo => repo.DeleteAsync(command.OrganisationId, command.Id, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _qrCodeTargetRepositoryServiceMock
            .Setup(repo => repo.DeleteAsync(command.Id, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(Unit.Value);

        _qrCodeRepositoryServiceMock.Verify(repo =>
            repo.DeleteAsync(command.OrganisationId, command.Id, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_RepositoryThrowsException_PropagatesException()
    {
        // Arrange
        var command = new Command
        {
            Id = "qr123",
            OrganisationId = "org456"
        };

        _qrCodeRepositoryServiceMock
            .Setup(repo => repo.DeleteAsync(command.OrganisationId, command.Id, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Repository exception"));

        _qrCodeTargetRepositoryServiceMock
            .Setup(repo => repo.DeleteAsync(command.Id, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Repository exception"));

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Repository exception");
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
    public async Task Handle_EmptyCommandProperties_CallsRepository()
    {
        // Arrange
        var command = new Command
        {
            Id = string.Empty,
            OrganisationId = string.Empty
        };

        _qrCodeRepositoryServiceMock
            .Setup(repo => repo.DeleteAsync(command.OrganisationId, command.Id, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _qrCodeTargetRepositoryServiceMock
            .Setup(repo => repo.DeleteAsync(command.Id, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(Unit.Value);

        _qrCodeRepositoryServiceMock.Verify(repo =>
            repo.DeleteAsync(command.OrganisationId, command.Id, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}