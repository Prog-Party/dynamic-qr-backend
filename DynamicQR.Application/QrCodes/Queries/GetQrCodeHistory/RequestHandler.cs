using DynamicQR.Domain.Interfaces;
using MediatR;

namespace DynamicQR.Application.QrCodes.Queries.GetQrCodeHistory;

public class RequestHandler : IRequestHandler<Request, List<Response>>
{
    private readonly IQrCodeHistoryRepositoryService _qrCodeHistoryRepositoryService;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="qrCodeHistoryRepositoryService"></param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="qrCodeHistoryRepositoryService"/> is null.</exception>
    public RequestHandler(IQrCodeHistoryRepositoryService qrCodeHistoryRepositoryService)
    {
        _qrCodeHistoryRepositoryService = qrCodeHistoryRepositoryService ?? throw new ArgumentNullException(nameof(qrCodeHistoryRepositoryService));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is null.</exception>
    /// <exception cref="Azure.RequestFailedException"></exception>
    /// <returns></returns>
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