using System.Drawing;

namespace DynamicQR.Domain.Models;

public class QrCode
{
    /// <summary>
    /// This Id is part of the web url used for dynamic nature
    /// </summary>
    public string Id { get; set; } = "0";
    public bool IncludeMargin { get; set; }
    public Color BackgroundColor { get; set; } = Color.White;
    public Color ForegroundColor { get; set; } = Color.Black;
    public string? ImageUrl { get; set; }
    public int? ImageHeight { get; set; }
    public int? ImageWidth { get; set; }
}