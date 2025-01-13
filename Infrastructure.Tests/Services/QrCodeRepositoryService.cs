using Azure;
using Azure.Core;
using Azure.Data.Tables;
using DynamicQR.Domain.Exceptions;
using DynamicQR.Domain.Models;
using DynamicQR.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Azure.Storage;
using Moq;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Infrastructure.Tests.Services;

[ExcludeFromCodeCoverage]
public sealed class QrCodeRepositoryService
{
    private readonly Mock<TableClient> _tableClientMock;
    private readonly Mock<TableServiceClient> _tableServiceClientMock;
    private readonly DynamicQR.Infrastructure.Services.QrCodeRepositoryService _repositoryService;

    public QrCodeRepositoryService()
    {
        _tableClientMock = new Mock<TableClient>();
        _tableServiceClientMock = new Mock<TableServiceClient>();

        _tableServiceClientMock
            .Setup(client => client.GetTableClient(It.IsAny<string>()))
            .Returns(_tableClientMock.Object);

        _repositoryService = new DynamicQR.Infrastructure.Services.QrCodeRepositoryService(_tableServiceClientMock.Object);
    }

    [Fact]
    public void Constructor_NullTableServiceClient_ShouldThrowArgumentNullException()
    {
        // Act
        Action act = () => new DynamicQR.Infrastructure.Services.QrCodeRepositoryService(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'tableServiceClient')");
    }

    [Fact]
    public async Task CreateAsync_ValidData_ShouldAddEntity()
    {
        // Arrange
        var organizationId = "org123";
        var qrCode = new QrCode { Id = "qr123" };

        _tableClientMock
            .Setup(client => client.AddEntityAsync(It.IsAny<DynamicQR.Infrastructure.Entities.QrCode>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Azure.Response.FromValue(new Mock<Azure.Response>().Object, new Mock<Azure.Response>().Object));

        // Act
        await _repositoryService.CreateAsync(organizationId, qrCode, CancellationToken.None);

        // Assert
        _tableClientMock.Verify(client => client.AddEntityAsync(It.IsAny<DynamicQR.Infrastructure.Entities.QrCode>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_NullOrganizationId_ShouldThrowArgumentNullException()
    {
        // Arrange
        var qrCode = new QrCode { Id = "qr123" };

        // Act
        Func<Task> act = async () => await _repositoryService.CreateAsync(null!, qrCode, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'organizationId')");
    }

    [Fact]
    public async Task CreateAsync_NullQrCode_ShouldThrowArgumentNullException()
    {
        // Arrange
        var organizationId = "org123";

        // Act
        Func<Task> act = async () => await _repositoryService.CreateAsync(organizationId, null!, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'qrCode')");
    }

    [Fact]
    public async Task ReadAsync_ValidId_ShouldReturnQrCode()
    {
        // Arrange
        var organizationId = "org123";
        var id = "qr123";
        var qrCodeEntity = new DynamicQR.Infrastructure.Entities.QrCode { PartitionKey = organizationId, RowKey = id };

        _tableClientMock
            .Setup(client => client.GetEntityIfExistsAsync<DynamicQR.Infrastructure.Entities.QrCode>(organizationId, id, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Azure.Response.FromValue(qrCodeEntity, new Mock<Azure.Response>().Object));

        // Act
        var result = await _repositoryService.ReadAsync(organizationId, id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();

        _tableClientMock.Verify(client => client.GetEntityIfExistsAsync<DynamicQR.Infrastructure.Entities.QrCode>(organizationId, id, null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ReadAsync_NotFound_ShouldThrowStorageException()
    {
        // Arrange
        var organizationId = "org123";
        var id = "nonexistentId";

        _tableClientMock
            .Setup(client => client.GetEntityIfExistsAsync<DynamicQR.Infrastructure.Entities.QrCode>(organizationId, id, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MockNullableResponse<DynamicQR.Infrastructure.Entities.QrCode>(404));

        // Act
        Func<Task> act = async () => await _repositoryService.ReadAsync(organizationId, id, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<StorageException>();

        _tableClientMock.Verify(client => client.GetEntityIfExistsAsync<DynamicQR.Infrastructure.Entities.QrCode>(organizationId, id, null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ValidData_ShouldUpdateEntity()
    {
        // Arrange
        var organizationId = "org123";
        var qrCode = new QrCode { Id = "qr123" };
        var qrCodeEntity = new DynamicQR.Infrastructure.Entities.QrCode { PartitionKey = organizationId, RowKey = qrCode.Id, ImageUrl = "OldValue", ETag = new ETag("*") };

        _tableClientMock
            .Setup(client => client.GetEntityIfExistsAsync<DynamicQR.Infrastructure.Entities.QrCode>(organizationId, qrCode.Id, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Azure.Response.FromValue(qrCodeEntity, new Mock<Azure.Response>().Object));

        _tableClientMock
            .Setup(client => client.UpdateEntityAsync(It.IsAny<DynamicQR.Infrastructure.Entities.QrCode>(), It.IsAny<ETag>(), TableUpdateMode.Merge, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Azure.Response.FromValue(new Mock<Azure.Response>().Object, new Mock<Azure.Response>().Object));

        // Act
        await _repositoryService.UpdateAsync(organizationId, qrCode, CancellationToken.None);

        // Assert
        _tableClientMock.Verify(client => client.UpdateEntityAsync(It.IsAny<DynamicQR.Infrastructure.Entities.QrCode>(), It.IsAny<ETag>(), TableUpdateMode.Merge, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_NotFound_ShouldThrowQrCodeNotFoundException()
    {
        // Arrange
        var organizationId = "org123";
        var id = "nonexistentId";

        _tableClientMock
            .Setup(client => client.GetEntityIfExistsAsync<DynamicQR.Infrastructure.Entities.QrCode>(organizationId, id, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MockNullableResponse<DynamicQR.Infrastructure.Entities.QrCode>(404));

        // Act
        Func<Task> act = async () => await _repositoryService.DeleteAsync(organizationId, id, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<QrCodeNotFoundException>();

        _tableClientMock.Verify(client => client.GetEntityIfExistsAsync<DynamicQR.Infrastructure.Entities.QrCode>(organizationId, id, null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_NotFound_ShouldThrowStorageException()
    {
        // Arrange
        var organizationId = "org123";
        var id = "nonexistentId";

        //// Create a mock of Azure.Response
        //var mockResponse = new Mock<Response>();

        //// Setup the IsError property to return true
        //mockResponse.Setup(response => response.IsError).Returns(true);

        //// You can also mock other members of the Response class as needed
        //mockResponse.Setup(response => response.Status).Returns(500); // Example status code
        //mockResponse.Setup(response => response.ReasonPhrase).Returns("Internal Server Error");

        //// Use the mocked Response object
        //Response mockedResponse = mockResponse.Object;

        DynamicQR.Infrastructure.Entities.QrCode qrCode = new()
        {
            IncludeMargin = true,
            BackgroundColor = "#FFFFFF",
            ForegroundColor = "#000000",
            ImageUrl = string.Empty,
            ImageHeight = 256,
            ImageWidth = 256,
            PartitionKey = "PartitionKey",
            RowKey = "RowKey"
        };

        var mockResponse = new Mock<Response>();

        // Configure the mock response to simulate an error
        mockResponse
            .Setup(r => r.IsError)
            .Returns(true);
        mockResponse
            .Setup(r => r.ReasonPhrase)
            .Returns("Mocked Error: Entity not found.");

        _tableClientMock
            .Setup(client => client.GetEntityIfExistsAsync<DynamicQR.Infrastructure.Entities.QrCode>(organizationId, id, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Azure.Response.FromValue(qrCode, new Mock<Azure.Response>().Object));

        _tableClientMock
            .Setup(tc => tc.DeleteEntityAsync(
                It.IsAny<string>(), // PartitionKey
                It.IsAny<string>(), // RowKey
                It.IsAny<ETag>(),   // ETag
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse.Object);

        // Act
        Func<Task> act = async () => await _repositoryService.DeleteAsync(organizationId, id, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<StorageException>();

        _tableClientMock.Verify(client => client.GetEntityIfExistsAsync<DynamicQR.Infrastructure.Entities.QrCode>(organizationId, id, null, It.IsAny<CancellationToken>()), Times.Once);
        _tableClientMock.Verify(client => client.DeleteEntityAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ETag>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    public class MockNullableResponse<T> : NullableResponse<T>
    {
        private readonly int _status;

        public MockNullableResponse(int status)
        {
            _status = status;
        }

        public override bool HasValue => false;

        public override T? Value => default;

        public override Response GetRawResponse()
        {
            throw new NotImplementedException();
        }
    }
}