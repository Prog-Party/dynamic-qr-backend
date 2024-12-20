using MediatR;

namespace DynamicQR.Application.QrCodes.Queries.GetAllQrCodes;

public sealed record Request : IRequest<List<Response>>
{
    public string OrganizationId { get; init; } = string.Empty;
}
