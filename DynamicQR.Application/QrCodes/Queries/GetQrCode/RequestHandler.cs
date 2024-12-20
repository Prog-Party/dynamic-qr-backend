using DynamicQR.Domain.Interfaces;
using MediatR;

namespace DynamicQR.Application.QrCodes.Queries.GetQrCode;

public class RequestHandler : IRequestHandler<Request, Response>
{
    private readonly IQrCodeRepositoryService _qrCodeRepositoryService;

    public RequestHandler(IQrCodeRepositoryService qrCodeRepositoryService)
    {
        _qrCodeRepositoryService = qrCodeRepositoryService ?? throw new ArgumentNullException(nameof(qrCodeRepositoryService));
    }

    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var qrCode = await _qrCodeRepositoryService.ReadAsync(request.OrganizationId, request.Id, cancellationToken);

        return new Response
        {
            BackgroundColor = qrCode.BackgroundColor,
            ForegroundColor = qrCode.ForegroundColor,
            Id = qrCode.Id,
            ImageHeight = qrCode.ImageHeight,
            ImageWidth = qrCode.ImageWidth,
            ImageUrl = qrCode.ImageUrl,
            IncludeMargin = qrCode.IncludeMargin
        };
    }
}