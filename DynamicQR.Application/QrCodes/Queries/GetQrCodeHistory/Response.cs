namespace DynamicQR.Application.QrCodes.Queries.GetQrCodeHistory;

public sealed record Response
{
    public string QrCodeId { get; init; } = string.Empty;
    public string Order { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    public string CustomerId { get; init; } = string.Empty;
    public string OrganizationId { get; init; } = string.Empty;
    public string EventType { get; init; } = string.Empty;
    public Dictionary<string, string> Details { get; init; } = new();
}