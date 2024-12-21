using Azure;
using Azure.Data.Tables;

namespace DynamicQR.Infrastructure.Entities;

public class QrCodeHistory : ITableEntity
{
    public string PartitionKey { get; set; } = string.Empty;
    public string RowKey { get; set; } = string.Empty;
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    public string CustomerId { get; set; } = string.Empty;
    public string OrganizationId { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
}
