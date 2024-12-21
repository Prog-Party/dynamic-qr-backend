using System.Text.Json;
using DynamicQR.Domain.Models;
using DynamicQR.Infrastructure.Entities;

namespace DynamicQR.Infrastructure.Mappers;

public static class QrCodeHistoryMappers
{
    public static DynamicQR.Domain.Models.QrCodeHistory ToCore(this DynamicQR.Infrastructure.Entities.QrCodeHistory entity)
        => new DynamicQR.Domain.Models.QrCodeHistory
        {
            Order = entity.RowKey,
            Timestamp = entity.Timestamp.UtcDateTime,
            CustomerId = entity.CustomerId,
            OrganizationId = entity.OrganizationId,
            EventType = entity.EventType,
            Details = JsonSerializer.Deserialize<Dictionary<string, string>>(entity.Details) ?? new Dictionary<string, string>()
        };

    public static DynamicQR.Infrastructure.Entities.QrCodeHistory ToInfrastructure(this DynamicQR.Domain.Models.QrCodeHistory model)
        => new DynamicQR.Infrastructure.Entities.QrCodeHistory
        {
            PartitionKey = model.OrganizationId,
            RowKey = model.Order,
            Timestamp = new DateTimeOffset(model.Timestamp),
            CustomerId = model.CustomerId,
            OrganizationId = model.OrganizationId,
            EventType = model.EventType,
            Details = JsonSerializer.Serialize(model.Details)
        };
}
