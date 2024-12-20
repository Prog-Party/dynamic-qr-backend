using MediatR;

namespace DynamicQR.Application.QrCodes.Queries.GetQrCodeTarget;

public sealed record Request : IRequest<Response>
{
    public string Id { get; init; } = string.Empty;
}