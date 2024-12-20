using DynamicQR.Domain.Models;

namespace DynamicQR.Domain.Interfaces;

public interface IQrCodeRepositoryService
{
    public Task CreateAsync(string organisationId, QrCode qrCode, CancellationToken cancellationToken);

    public Task<QrCode> ReadAsync(string organisationId, string id, CancellationToken cancellationToken);

    public Task UpdateAsync(string organisationId, QrCode qrCode, CancellationToken cancellationToken);

    public Task DeleteAsync(string organisationId, string id, CancellationToken cancellationToken);

    public Task<List<QrCode>> GetAllAsync(string organizationId, CancellationToken cancellationToken);
}
