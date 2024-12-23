using DynamicQR.Domain.Interfaces;
using MediatR;

namespace DynamicQR.Application.QrCodes.Queries.GetQrCode;

public class RequestHandler : IRequestHandler<Request, Response>
{
    private readonly IQrCodeRepositoryService _qrCodeRepositoryService;
    private readonly IQrCodeTargetRepositoryService _qrCodeTargetRepositoryService;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="qrCodeRepositoryService"></param>
    /// <param name="qrCodeTargetRepositoryService"></param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="qrCodeRepositoryService"/> or <paramref name="qrCodeTargetRepositoryService"/> is null.</exception>
    public RequestHandler(IQrCodeRepositoryService qrCodeRepositoryService, IQrCodeTargetRepositoryService qrCodeTargetRepositoryService)
    {
        _qrCodeRepositoryService = qrCodeRepositoryService ?? throw new ArgumentNullException(nameof(qrCodeRepositoryService));
        _qrCodeTargetRepositoryService = qrCodeTargetRepositoryService ?? throw new ArgumentNullException(nameof(qrCodeTargetRepositoryService));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is null.</exception>
    /// <exception cref="Azure.RequestFailedException"></exception>
    /// <returns></returns>
    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var qrCode = await _qrCodeRepositoryService.ReadAsync(request.OrganizationId, request.Id, cancellationToken);
        var qrcodeTarget = await _qrCodeTargetRepositoryService.ReadAsync(request.Id, cancellationToken);

        return new Response
        {
            BackgroundColor = qrCode.BackgroundColor,
            ForegroundColor = qrCode.ForegroundColor,
            Id = qrCode.Id,
            Value = qrcodeTarget.Value,
            ImageHeight = qrCode.ImageHeight,
            ImageWidth = qrCode.ImageWidth,
            ImageUrl = qrCode.ImageUrl,
            IncludeMargin = qrCode.IncludeMargin
        };
    }
}