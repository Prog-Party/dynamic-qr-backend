using MediatR;

namespace DynamicQR.Application.QrCodes.Commands.UpdateQrCodeTarget;

public sealed record Command : IRequest<Response>
{
    public string Id { get; init; } = string.Empty;
    public string Value { get; init; } = string.Empty;
}