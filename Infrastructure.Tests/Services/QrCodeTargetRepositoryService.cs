using Azure;
using Azure.Data.Tables;
using DynamicQR.Domain.Models;
using DynamicQR.Infrastructure.Services;
using FluentAssertions;
using Moq;
using System.Diagnostics.CodeAnalysis;
using QrCodeTargetEntity = DynamicQR.Infrastructure.Entities.QrCodeTarget;

namespace Infrastructure.Tests.Services;

[ExcludeFromCodeCoverage]
public sealed class QrCodeTargetRepositoryService
{
    private readonly Mock<TableServiceClient> _tableServiceClientMock;
    private readonly Mock<TableClient> _tableClientMock;
    private readonly DynamicQR.Infrastructure.Services.QrCodeTargetRepositoryService _service;

    public QrCodeTargetRepositoryService()
    {
        _tableClientMock = new Mock<TableClient>();
        _tableServiceClientMock = new Mock<TableServiceClient>();

        _tableServiceClientMock
            .Setup(client => client.GetTableClient(It.IsAny<string>()))
            .Returns(_tableClientMock.Object);

        _service = new DynamicQR.Infrastructure.Services.QrCodeTargetRepositoryService(_tableServiceClientMock.Object);
    }

    [Fact]
    public void Constructor_NullTableServiceClient_ShouldThrowArgumentNullException()
    {
        // Act
        Action act = () => new DynamicQR.Infrastructure.Services.QrCodeTargetRepositoryService(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'tableServiceClient')");
    }

    [Fact]
    public async Task CreateAsync_ValidQrCodeTarget_ShouldCallAddEntityAsync()
    {
        // Arrange
        var qrCodeTarget = new QrCodeTarget { QrCodeId = "123", Value = "TestValue" };
        var qrCodeTargetEntity = new QrCodeTargetEntity { PartitionKey = "123", RowKey = "123", Value = "TestValue" };

        _tableClientMock
            .Setup(client => client.AddEntityAsync(qrCodeTargetEntity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Mock<Response>().Object);

        // Act
        await _service.CreateAsync(qrCodeTarget, CancellationToken.None);

        // Assert
        _tableClientMock.Verify(client => client.AddEntityAsync(It.Is<QrCodeTargetEntity>(
            entity => entity.PartitionKey == "123" && entity.RowKey == "123"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ReadAsync_EntityExists_ShouldReturnQrCodeTarget()
    {
        // Arrange
        var id = "123";
        var qrCodeTargetEntity = new QrCodeTargetEntity { PartitionKey = id, RowKey = id, Value = "TestValue" };

        _tableClientMock
            .Setup(client => client.GetEntityIfExistsAsync<QrCodeTargetEntity>(id, id, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Response.FromValue(qrCodeTargetEntity, new Mock<Response>().Object));

        // Act
        var result = await _service.ReadAsync(id, CancellationToken.None);

        // Assert
        result.QrCodeId.Should().Be(id);
        result.Value.Should().Be("TestValue");
    }

    [Fact]
    public async Task UpdateAsync_EntityExists_ShouldUpdateEntity()
    {
        // Arrange
        var qrCodeTarget = new QrCodeTarget { QrCodeId = "123", Value = "UpdatedValue" };
        var existingEntity = new QrCodeTargetEntity { PartitionKey = "123", RowKey = "123", Value = "OriginalValue", ETag = new ETag("*") };
        var updatedEntity = new QrCodeTargetEntity { PartitionKey = "123", RowKey = "123", Value = "UpdatedValue", ETag = new ETag("*") };

        _tableClientMock
            .Setup(client => client.GetEntityIfExistsAsync<QrCodeTargetEntity>("123", qrCodeTarget.QrCodeId, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Response.FromValue(existingEntity, new Mock<Response>().Object));

        _tableClientMock
            .Setup(client => client.UpdateEntityAsync(updatedEntity, It.IsAny<ETag>(), TableUpdateMode.Merge, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Mock<Response>().Object);

        // Act
        await _service.UpdateAsync(qrCodeTarget, CancellationToken.None);

        // Assert
        _tableClientMock.Verify(client => client.UpdateEntityAsync(It.Is<QrCodeTargetEntity>(
            entity => entity.Value == "UpdatedValue"), It.IsAny<ETag>(), TableUpdateMode.Merge, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_EntityExists_ShouldDeleteEntity()
    {
        // Arrange
        var id = "123";
        var entityToDelete = new QrCodeTargetEntity { PartitionKey = id, RowKey = id, ETag = new ETag("*") };

        _tableClientMock
            .Setup(client => client.GetEntityIfExistsAsync<QrCodeTargetEntity>(id, id, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Response.FromValue(entityToDelete, new Mock<Response>().Object));

        _tableClientMock
            .Setup(client => client.DeleteEntityAsync(id, id, It.IsAny<ETag>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Mock<Response>().Object);

        // Act
        await _service.DeleteAsync(id, CancellationToken.None);

        // Assert
        _tableClientMock.Verify(client => client.DeleteEntityAsync(
            id, id, It.IsAny<ETag>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Exists_ShouldReturnTrueIfQrCodeIdExists()
    {
        // Arrange
        string qrCodeId = "existing-id";
        _tableClientMock
            .Setup(client => client.GetEntityIfExistsAsync<QrCodeTargetEntity>(qrCodeId, qrCodeId, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Response.FromValue(new QrCodeTargetEntity(), new Mock<Response>().Object));

        // Act
        bool result = await _service.Exists(qrCodeId, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task Exists_ShouldReturnFalseIfQrCodeIdDoesNotExist()
    {
        // Arrange
        string qrCodeId = "non-existing-id";
        _tableClientMock
            .Setup(client => client.GetEntityIfExistsAsync<QrCodeTargetEntity>(qrCodeId, qrCodeId, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Response.FromValue<QrCodeTargetEntity>(null, new Mock<Response>().Object));

        // Act
        bool result = await _service.Exists(qrCodeId, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }
}