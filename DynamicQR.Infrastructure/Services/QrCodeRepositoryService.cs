using Azure.Data.Tables;
using DynamicQR.Domain.Exceptions;
using DynamicQR.Domain.Interfaces;
using DynamicQR.Domain.Models;
using DynamicQR.Infrastructure.Mappers;
using Microsoft.Azure.Storage;

namespace DynamicQR.Infrastructure.Services;

public sealed class QrCodeRepositoryService : IQrCodeRepositoryService
{
    private readonly TableClient _tableClient;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tableServiceClient"></param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="tableServiceClient"/> is null.</exception>
    public QrCodeRepositoryService(TableServiceClient tableServiceClient)
    {
        ArgumentNullException.ThrowIfNull(tableServiceClient);

        _tableClient = tableServiceClient.GetTableClient(tableName: "qrcodes");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="organizationId"></param>
    /// <param name="qrCode"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="StorageException"></exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="qrCode"/> or <paramref name="organizationId"/> is null.</exception>
    /// <exception cref="Azure.RequestFailedException"></exception>
    public async Task CreateAsync(string organizationId, QrCode qrCode, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(organizationId);
        ArgumentNullException.ThrowIfNull(qrCode);

        Entities.QrCode qrCodeData = qrCode.ToInfrastructure(organizationId);

        Azure.Response response = await _tableClient.AddEntityAsync(qrCodeData, cancellationToken);

        if (response.IsError)
            throw new StorageException(response.ReasonPhrase);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="organisationId"></param>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="StorageException"></exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="id"/> or <paramref name="organisationId"/> is null.</exception>
    /// <exception cref="Azure.RequestFailedException"></exception>
    public async Task<QrCode> ReadAsync(string organisationId, string id, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(organisationId);
        ArgumentNullException.ThrowIfNull(id);

        Azure.NullableResponse<Entities.QrCode> data = await _tableClient.GetEntityIfExistsAsync<Entities.QrCode>(organisationId, id, cancellationToken: cancellationToken);

        if (data.HasValue)
            return data.Value!.ToCore();

        throw new StorageException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="organisationId"></param>
    /// <param name="qrCode"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="QrCodeNotFoundException"></exception>
    /// <exception cref="StorageException"></exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="organisationId"/> or <paramref name="qrCode"/> is null.</exception>
    /// <exception cref="Azure.RequestFailedException"></exception>
    public async Task UpdateAsync(string organisationId, QrCode qrCode, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(organisationId);
        ArgumentNullException.ThrowIfNull(qrCode);

        Entities.QrCode qrCodeInput = QrCodeMappers.ToInfrastructure(qrCode, organisationId);

        Azure.NullableResponse<Entities.QrCode> data = await _tableClient.GetEntityIfExistsAsync<Entities.QrCode>(organisationId, qrCode.Id, cancellationToken: cancellationToken);

        if (!data.HasValue)
            throw new QrCodeNotFoundException(organisationId, qrCode.Id);

        Entities.QrCode qrCodeToUpdate = data.Value!;

        qrCodeToUpdate.IncludeMargin = qrCodeInput.IncludeMargin;
        qrCodeToUpdate.ForegroundColor = qrCodeInput.ForegroundColor;
        qrCodeToUpdate.BackgroundColor = qrCodeInput.BackgroundColor;
        qrCodeToUpdate.ImageHeight = qrCodeInput.ImageHeight;
        qrCodeToUpdate.ImageWidth = qrCodeInput.ImageWidth;
        qrCodeToUpdate.ImageUrl = qrCodeInput.ImageUrl;

        Azure.Response response = await _tableClient.UpdateEntityAsync(qrCodeToUpdate, qrCodeToUpdate.ETag, TableUpdateMode.Merge, cancellationToken);

        if (response.IsError)
            throw new StorageException(response.ReasonPhrase);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="organisationId"></param>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="QrCodeNotFoundException"></exception>
    /// <exception cref="StorageException"></exception>
    /// <exception cref="Azure.RequestFailedException"></exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="organisationId"/> or <paramref name="id"/> is null.</exception>
    public async Task DeleteAsync(string organisationId, string id, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(organisationId);
        ArgumentNullException.ThrowIfNull(id);

        Azure.NullableResponse<Entities.QrCode> data = await _tableClient.GetEntityIfExistsAsync<Entities.QrCode>(organisationId, id, cancellationToken: cancellationToken);

        if (!data.HasValue)
            throw new QrCodeNotFoundException(organisationId, id);

        Entities.QrCode qrCodeToDelete = data.Value!;

        Azure.Response response = await _tableClient.DeleteEntityAsync(qrCodeToDelete.PartitionKey, qrCodeToDelete.RowKey, qrCodeToDelete.ETag, cancellationToken);

        if (response.IsError)
            throw new StorageException(response.ReasonPhrase);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="organizationId"></param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="organizationId"/> is null.</exception>
    /// <exception cref="Azure.RequestFailedException"></exception>
    /// <returns>A list of qr codes.</returns>
    public async Task<List<QrCode>> GetAllAsync(string organizationId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(organizationId);

        var qrCodes = new List<QrCode>();

        await foreach (var qrCodeEntity in _tableClient.QueryAsync<Entities.QrCode>(qrCode => qrCode.PartitionKey == organizationId, cancellationToken: cancellationToken))
        {
            qrCodes.Add(qrCodeEntity.ToCore());
        }

        return qrCodes;
    }
}
