using DynamicQR.Domain.Interfaces;
using DynamicQR.Domain.Models;
using MediatR;

namespace DynamicQR.Application.QrCodes.Commands.UpdateQrCodeTarget;

public class CommandHandler : IRequestHandler<Command, Response>
{
    private readonly IQrCodeTargetRepositoryService _qrCodeRepositoryService;
    private readonly IQrCodeHistoryRepositoryService _qrCodeHistoryRepositoryService;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="qrCodeTargetRepositoryService"></param>
    /// <param name="qrCodeHistoryRepositoryService"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public CommandHandler(IQrCodeTargetRepositoryService qrCodeTargetRepositoryService, IQrCodeHistoryRepositoryService qrCodeHistoryRepositoryService)
    {
        _qrCodeRepositoryService = qrCodeTargetRepositoryService ?? throw new ArgumentNullException(nameof(qrCodeTargetRepositoryService));
        _qrCodeHistoryRepositoryService = qrCodeHistoryRepositoryService ?? throw new ArgumentNullException(nameof(qrCodeHistoryRepositoryService));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns></returns>
    public async Task<Response> Handle(Command command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        QrCodeTarget qrCodeTarget = new()
        {
            QrCodeId = command.Id,
            Value = command.Value,
        };

        await LogHistory(command, cancellationToken);
        await _qrCodeRepositoryService.UpdateAsync(qrCodeTarget, cancellationToken);

        return new Response { Id = command.Id };
    }

    private async Task LogHistory(Command command, CancellationToken cancellationToken)
    {
        Dictionary<string, string> details = new()
        {
            { "NewValue", command.Value }
        };

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