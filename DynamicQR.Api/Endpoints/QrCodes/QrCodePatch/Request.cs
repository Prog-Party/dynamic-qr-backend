namespace DynamicQR.Api.Endpoints.QrCodes.QrCodePatch;

public sealed record QrCodePatchRequest
{
    public bool IncludeMargin { get; init; }
    public string BackgroundColor { get; init; } = "#FFF";
    public string ForegroundColor { get; init; } = "#000";
    public string? ImageUrl { get; init; }
    public int? ImageHeight { get; init; }
    public int? ImageWidth { get; init; }
}