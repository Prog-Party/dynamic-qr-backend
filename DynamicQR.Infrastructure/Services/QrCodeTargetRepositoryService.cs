using Azure.Data.Tables;
using DynamicQR.Domain.Exceptions;
using DynamicQR.Domain.Interfaces;
using DynamicQR.Domain.Models;
using DynamicQR.Infrastructure.Mappers;
using Microsoft.Azure.Storage;

namespace DynamicQR.Infrastructure.Services;

public sealed class QrCodeTargetRepositoryService : IQrCodeTargetRepositoryService
{
    private readonly TableClient _tableClient;

    public QrCodeTargetRepositoryService(TableServiceClient tableServiceClient)
    {
        ArgumentNullException.ThrowIfNull(tableServiceClient);

        _tableClient = tableServiceClient.GetTableClient(tableName: "qrcodetargets");
    }

    public async Task CreateAsync(QrCodeTarget qrCodeTarget, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(qrCodeTarget);

        var qrCodeTargetData = qrCodeTarget.ToInfrastructure();

        Azure.Response response = await _tableClient.AddEntityAsync(qrCodeTargetData, cancellationToken);

        if (response.IsError)
            throw new StorageException(response.ReasonPhrase);
    }

    public async Task<QrCodeTarget> ReadAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(id);

        Azure.NullableResponse<Entities.QrCodeTarget> data = await _tableClient.GetEntityIfExistsAsync<Entities.QrCodeTarget>(id, id, cancellationToken: cancellationToken);

        if (data.HasValue)
            return data.Value!.ToCore();

        throw new StorageException();
    }

    public async Task UpdateAsync(QrCodeTarget qrCodeTarget, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(qrCodeTarget);
        Entities.QrCodeTarget qrCodeTargetInput = QrCodeTargetMappers.ToInfrastructure(qrCodeTarget);

        Azure.NullableResponse<Entities.QrCodeTarget> data = await _tableClient.GetEntityIfExistsAsync<Entities.QrCodeTarget>(qrCodeTarget.QrCodeId, qrCodeTarget.QrCodeId, cancellationToken: cancellationToken);

        if (!data.HasValue)
            throw new QrCodeTargetNotFoundException(qrCodeTarget.QrCodeId);

        Entities.QrCodeTarget qrCodeToUpdate = data.Value!;

        qrCodeToUpdate.Value = qrCodeTargetInput.Value;

        Azure.Response response = await _tableClient.UpdateEntityAsync(qrCodeToUpdate, qrCodeToUpdate.ETag, TableUpdateMode.Merge, cancellationToken);

        if (response.IsError)
            throw new StorageException(response.ReasonPhrase);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(id);

        Azure.NullableResponse<Entities.QrCodeTarget> data = await _tableClient.GetEntityIfExistsAsync<Entities.QrCodeTarget>(id, id, cancellationToken: cancellationToken);

        if (!data.HasValue)
            throw new QrCodeTargetNotFoundException(id);

        Entities.QrCodeTarget qrCodeTargetToDelete = data.Value!;

        Azure.Response response = await _tableClient.DeleteEntityAsync(qrCodeTargetToDelete.PartitionKey, qrCodeTargetToDelete.RowKey, qrCodeTargetToDelete.ETag, cancellationToken);

        if (response.IsError)
            throw new StorageException(response.ReasonPhrase);
    }

    public async Task<bool> Exists(string id, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(id);

        Azure.NullableResponse<Entities.QrCodeTarget> data = await _tableClient.GetEntityIfExistsAsync<Entities.QrCodeTarget>(id, id, cancellationToken: cancellationToken);

        return data.HasValue && data.Value != null;
    }
}
