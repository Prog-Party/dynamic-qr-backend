using DynamicQR.Domain.Models;

namespace DynamicQR.Domain.Interfaces;

public interface IQrCodeHistoryRepositoryService
{
    Task<List<QrCodeHistory>> GetHistoryAsync(string qrCodeId, CancellationToken cancellationToken);
    Task AddHistoryAsync(QrCodeHistory historyItem, CancellationToken cancellationToken);
}
