using System.Drawing;

namespace DynamicQR.Application.QrCodes.Queries.GetAllQrCodes;

public sealed record Response
{
    public string Id { get; init; } = string.Empty;
    public Color BackgroundColor { get; init; }
    public Color ForegroundColor { get; init; }
    public string? ImageUrl { get; init; }
    public int? ImageHeight { get; init; }
    public int? ImageWidth { get; init; }
    public bool IncludeMargin { get; init; }
}
