namespace DynamicQR.Domain.Models;

public sealed record QrCodeHistory
{
    public string Order { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    public string CustomerId { get; init; } = string.Empty;
    public string OrganizationId { get; init; } = string.Empty;
    public string EventType { get; init; } = string.Empty;
    public Dictionary<string, string> Details { get; init; } = new();
}
