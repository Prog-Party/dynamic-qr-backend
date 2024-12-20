using MediatR;
using System.Drawing;

namespace DynamicQR.Application.QrCodes.Commands.UpdateQrCode;

public sealed record Command : IRequest<Response>
{
    public string Id { get; init; } = string.Empty;
    public bool IncludeMargin { get; init; }
    public Color BackgroundColor { get; init; }
    public Color ForegroundColor { get; init; }
    public string? ImageUrl { get; init; }
    public int? ImageHeight { get; init; }
    public int? ImageWidth { get; init; }
    public string OrganisationId { get; init; } = string.Empty;
}