using DynamicQR.Domain.Interfaces;
using DynamicQR.Domain.Models;
using MediatR;

namespace DynamicQR.Application.QrCodes.Commands.DeleteQrCode;

public class CommandHandler : IRequestHandler<Command, Unit>
{
    private readonly IQrCodeRepositoryService _qrCodeRepositoryService;
    private readonly IQrCodeTargetRepositoryService _qrCodeTargetRepositoryService;
    private readonly IQrCodeHistoryRepositoryService _qrCodeHistoryRepositoryService;

    public CommandHandler(IQrCodeRepositoryService qrCodeRepositoryService, IQrCodeTargetRepositoryService qrCodeTargetRepositoryService, IQrCodeHistoryRepositoryService qrCodeHistoryRepositoryService)
    {
        _qrCodeRepositoryService = qrCodeRepositoryService ?? throw new ArgumentNullException(nameof(qrCodeRepositoryService));
        _qrCodeTargetRepositoryService = qrCodeTargetRepositoryService ?? throw new ArgumentNullException(nameof(qrCodeTargetRepositoryService));
        _qrCodeHistoryRepositoryService = qrCodeHistoryRepositoryService ?? throw new ArgumentNullException(nameof(qrCodeHistoryRepositoryService));
    }

    public async Task<Unit> Handle(Command command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        await LogHistory(command, cancellationToken);
        await _qrCodeTargetRepositoryService.DeleteAsync(command.Id, cancellationToken);
        await _qrCodeRepositoryService.DeleteAsync(command.OrganizationId, command.Id, cancellationToken);

        return Unit.Value;
    }

    private async Task LogHistory(Command command, CancellationToken cancellationToken)
    {
        QrCodeHistory historyItem = new()
        {
            QrCodeId = command.Id,
            CustomerId = command.CustomerId,
            OrganizationId = command.OrganizationId,
            EventType = QrCodeEvents.Lifecycle.Deleted
        };

        await _qrCodeHistoryRepositoryService.AddHistoryAsync(historyItem, cancellationToken);
    }
}