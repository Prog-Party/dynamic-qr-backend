using DynamicQR.Domain.Models;

namespace DynamicQR.Domain.Interfaces;

public interface IQrCodeTargetRepositoryService
{
    public Task CreateAsync(QrCodeTarget qrCodeTarget, CancellationToken cancellationToken);

    public Task<QrCodeTarget> ReadAsync(string id, CancellationToken cancellationToken);

    public Task UpdateAsync(QrCodeTarget qrCodeTarget, CancellationToken cancellationToken);

    public Task DeleteAsync(string id, CancellationToken cancellationToken);

    public Task<bool> Exists(string id, CancellationToken cancellationToken);
}
