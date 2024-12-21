using MediatR;

namespace DynamicQR.Application.QrCodes.Queries.GetQrCodeHistory;

public sealed record Request : IRequest<List<Response>>
{
    public string QrCodeId { get; init; } = string.Empty;
    public string OrganizationId { get; init; } = string.Empty;
}