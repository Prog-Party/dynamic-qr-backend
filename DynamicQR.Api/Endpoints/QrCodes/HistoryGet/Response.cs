namespace DynamicQR.Api.Endpoints.QrCodes.HistoryGet;

public sealed record Response
{
    public string QrCodeId { get; init; } = string.Empty;
    public string Order { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; }
    public string CustomerId { get; init; } = string.Empty;
    public string OrganizationId { get; init; } = string.Empty;
    public string EventType { get; init; } = string.Empty;
    public Dictionary<string, string> Details { get; init; } = new();
}
