using MediatR;

namespace DynamicQR.Application.QrCodes.Commands.DeleteQrCode;

public sealed record Command : IRequest<Unit>
{
    public string Id { get; init; } = string.Empty;
    public string OrganisationId { get; init; } = string.Empty;
}