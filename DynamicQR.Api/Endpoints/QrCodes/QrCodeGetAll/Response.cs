namespace DynamicQR.Api.Endpoints.QrCodes.QrCodeGetAll;

public sealed record Response
{
    public string Id { get; init; } = string.Empty;
    public string Value { get; init; } = string.Empty;
    public string BackgroundColor { get; init; } = "#FFF";
    public string ForegroundColor { get; init; } = "#000";
    public string? ImageUrl { get; init; }
    public int? ImageHeight { get; init; }
    public int? ImageWidth { get; init; }
    public bool IncludeMargin { get; init; }
}
