using Azure;
using Azure.Data.Tables;

namespace DynamicQR.Infrastructure.Entities;

public sealed record QrCode : ITableEntity
{
    public bool IncludeMargin { get; set; }
    public string BackgroundColor { get; set; } = "#FFF";
    public string ForegroundColor { get; set; } = "#000";
    public string? ImageUrl { get; set; }
    public int? ImageHeight { get; set; }
    public int? ImageWidth { get; set; }
    public string PartitionKey { get; set; } = string.Empty;
    public string RowKey { get; set; } = string.Empty;
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}