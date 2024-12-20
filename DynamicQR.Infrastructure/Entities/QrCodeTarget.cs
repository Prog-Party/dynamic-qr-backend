﻿using Azure;
using Azure.Data.Tables;

namespace DynamicQR.Infrastructure.Entities;

public sealed record QrCodeTarget : ITableEntity
{
    public string Value { get; set; } = string.Empty;
    public string PartitionKey { get; set; } = string.Empty;
    public string RowKey { get; set; } = string.Empty;
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}