using DynamicQR.Domain.Interfaces;
using MediatR;

namespace DynamicQR.Application.QrCodes.Queries.GetQrCodeTarget;

public class RequestHandler : IRequestHandler<Request, Response>
{
    private readonly IQrCodeTargetRepositoryService _qrCodeTargetRepositoryService;

    public RequestHandler(IQrCodeTargetRepositoryService qrCodeRepositoryService)
    {
        _qrCodeTargetRepositoryService = qrCodeRepositoryService ?? throw new ArgumentNullException(nameof(qrCodeRepositoryService));
    }

    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var qrCodeTarget = await _qrCodeTargetRepositoryService.ReadAsync(request.Id, cancellationToken);

        return new Response
        {
            QrCodeId = qrCodeTarget.QrCodeId,
            Value = qrCodeTarget.Value,
        };
    }
}