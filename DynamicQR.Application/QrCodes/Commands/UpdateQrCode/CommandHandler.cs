using DynamicQR.Domain.Interfaces;
using DynamicQR.Domain.Models;
using MediatR;

namespace DynamicQR.Application.QrCodes.Commands.UpdateQrCode;

public class CommandHandler : IRequestHandler<Command, Response>
{
    private readonly IQrCodeRepositoryService _qrCodeRepositoryService;
    private readonly IQrCodeHistoryRepositoryService _qrCodeHistoryRepositoryService;

    public CommandHandler(IQrCodeRepositoryService qrCodeRepositoryService, IQrCodeHistoryRepositoryService qrCodeHistoryRepositoryService)
    {
        _qrCodeRepositoryService = qrCodeRepositoryService ?? throw new ArgumentNullException(nameof(qrCodeRepositoryService));
        _qrCodeHistoryRepositoryService = qrCodeHistoryRepositoryService;
    }

    public async Task<Response> Handle(Command command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        QrCode qrCode = new()
        {
            BackgroundColor = command.BackgroundColor,
            ForegroundColor = command.ForegroundColor,
            Id = command.Id,
            ImageHeight = command.ImageHeight,
            ImageUrl = command.ImageUrl,
            ImageWidth = command.ImageWidth,
            IncludeMargin = command.IncludeMargin,
        };

        await LogHistory(command, cancellationToken);
        await _qrCodeRepositoryService.UpdateAsync(command.OrganizationId, qrCode, cancellationToken);

        return new Response { Id = command.Id };
    }

    private async Task LogHistory(Command command, CancellationToken cancellationToken)
    {
        Dictionary<string, string> details = new()
        {
            { "IncludeMargin", command.IncludeMargin.ToString() },
            { "BackgroundColor", command.BackgroundColor.Name },
            { "ForegroundColor", command.ForegroundColor.Name },
        };
        if (!string.IsNullOrEmpty(command.ImageUrl))
            details.Add("ImageUrl", command.ImageUrl);
        if (command.ImageHeight.HasValue)
            details.Add("ImageHeight", command.ImageHeight.Value.ToString());
        if (command.ImageWidth.HasValue)
            details.Add("ImageWidth", command.ImageWidth.Value.ToString());

        QrCodeHistory historyItem = new()
        {
            QrCodeId = command.Id,
            CustomerId = command.CustomerId,
            OrganizationId = command.OrganizationId,
            EventType = QrCodeEvents.Lifecycle.Updated,
            Details = details
        };

        await _qrCodeHistoryRepositoryService.AddHistoryAsync(historyItem, cancellationToken);
    }
}