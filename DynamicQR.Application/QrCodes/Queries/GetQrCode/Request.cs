using MediatR;

namespace DynamicQR.Application.QrCodes.Queries.GetQrCode;

public sealed record Request : IRequest<Response>
{
    public string Id { get; init; } = string.Empty;
    public string OrganizationId { get; init; } = string.Empty;
}