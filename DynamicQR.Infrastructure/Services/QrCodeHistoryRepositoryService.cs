using Azure.Data.Tables;
using DynamicQR.Domain.Interfaces;
using DynamicQR.Domain.Models;
using DynamicQR.Infrastructure.Mappers;

namespace DynamicQR.Infrastructure.Services;

public sealed class QrCodeHistoryRepositoryService : IQrCodeHistoryRepositoryService
{
    private readonly TableClient _tableClient;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tableServiceClient"></param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="tableServiceClient"/> is null.</exception>
    public QrCodeHistoryRepositoryService(TableServiceClient tableServiceClient)
    {
        ArgumentNullException.ThrowIfNull(tableServiceClient);

        _tableClient = tableServiceClient.GetTableClient(tableName: "qrcodehistory");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="qrCodeId"></param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="qrCodeId"/> is null.</exception>
    /// <exception cref="Azure.RequestFailedException"></exception>
    /// <returns></returns>
    public async Task<List<QrCodeHistory>> GetHistoryAsync(string qrCodeId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(qrCodeId);

        var historyItems = new List<QrCodeHistory>();

        await foreach (var historyEntity in _tableClient.QueryAsync<Entities.QrCodeHistory>(history => history.PartitionKey == qrCodeId, cancellationToken: cancellationToken))
        {
            historyItems.Add(historyEntity.ToCore());
        }

        return historyItems;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="historyItem"></param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="historyItem"/> is null.</exception>
    /// <exception cref="Azure.RequestFailedException"></exception>
    /// <returns></returns>
    public async Task AddHistoryAsync(QrCodeHistory historyItem, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(historyItem);

        var historyEntity = historyItem.ToInfrastructure();
        historyEntity.RowKey = GenerateRowKey();

        await _tableClient.AddEntityAsync(historyEntity, cancellationToken);
    }

    private string GenerateRowKey()
        => $"{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid()}";
}
