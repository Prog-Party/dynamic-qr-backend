using DynamicQR.Domain.Interfaces;
using MediatR;

namespace DynamicQR.Application.QrCodes.Queries.GetQrCodeHistory;

public class RequestHandler : IRequestHandler<Request, List<Response>>
{
    private readonly IQrCodeHistoryRepositoryService _qrCodeHistoryRepositoryService;

    public RequestHandler(IQrCodeHistoryRepositoryService qrCodeHistoryRepositoryService)
    {
        _qrCodeHistoryRepositoryService = qrCodeHistoryRepositoryService ?? throw new ArgumentNullException(nameof(qrCodeHistoryRepositoryService));
    }

    public async Task<List<Response>> Handle(Request request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var histories = await _qrCodeHistoryRepositoryService.GetHistoryAsync(request.QrCodeId, cancellationToken);

        return histories.Select(x => new Response()
        {
            QrCodeId = request.QrCodeId,
            Order = x.Order,
            Timestamp = x.Timestamp,
            CustomerId = x.CustomerId,
            OrganizationId = request.OrganizationId,
            Details = x.Details,
            EventType = x.EventType
        }).ToList();
    }
}