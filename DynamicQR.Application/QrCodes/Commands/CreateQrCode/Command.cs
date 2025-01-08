using MediatR;
using System.Drawing;

namespace DynamicQR.Application.QrCodes.Commands.CreateQrCode;

public sealed record Command : IRequest<Response>
{
    public bool IncludeMargin { get; init; }
    public Color BackgroundColor { get; init; }
    public Color ForegroundColor { get; init; }
    public string? ImageUrl { get; init; }
    public int? ImageHeight { get; init; }
    public int? ImageWidth { get; init; }
    public string Value { get; init; } = string.Empty;
    public string OrganizationId { get; init; } = string.Empty;
    public string CustomerId { get; init; } = string.Empty;
}