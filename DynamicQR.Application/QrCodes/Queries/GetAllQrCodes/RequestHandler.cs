using DynamicQR.Domain.Interfaces;
using MediatR;

namespace DynamicQR.Application.QrCodes.Queries.GetAllQrCodes;

public class RequestHandler : IRequestHandler<Request, List<Response>>
{
    private readonly IQrCodeRepositoryService _qrCodeRepositoryService;

    public RequestHandler(IQrCodeRepositoryService qrCodeRepositoryService)
    {
        _qrCodeRepositoryService = qrCodeRepositoryService ?? throw new ArgumentNullException(nameof(qrCodeRepositoryService));
    }

    public async Task<List<Response>> Handle(Request request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var qrCodes = await _qrCodeRepositoryService.GetAllAsync(request.OrganizationId, cancellationToken);

        return qrCodes.Select(qrCode => new Response
        {
            Id = qrCode.Id,
            BackgroundColor = qrCode.BackgroundColor,
            ForegroundColor = qrCode.ForegroundColor,
            ImageUrl = qrCode.ImageUrl,
            ImageHeight = qrCode.ImageHeight,
            ImageWidth = qrCode.ImageWidth,
            IncludeMargin = qrCode.IncludeMargin
        }).ToList();
    }
}
