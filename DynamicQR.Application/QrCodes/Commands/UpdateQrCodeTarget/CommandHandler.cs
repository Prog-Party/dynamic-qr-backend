using DynamicQR.Domain.Interfaces;
using DynamicQR.Domain.Models;
using MediatR;

namespace DynamicQR.Application.QrCodes.Commands.UpdateQrCodeTarget;

public class CommandHandler : IRequestHandler<Command, Response>
{
    private readonly IQrCodeTargetRepositoryService _qrCodeRepositoryService;

    public CommandHandler(IQrCodeTargetRepositoryService qrCodeTargetRepositoryService)
    {
        _qrCodeRepositoryService = qrCodeTargetRepositoryService ?? throw new ArgumentNullException(nameof(qrCodeTargetRepositoryService));
    }

    public async Task<Response> Handle(Command command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        QrCodeTarget qrCodeTarget = new()
        {
            QrCodeId = command.Id,
            Value = command.Value,
        };

        await _qrCodeRepositoryService.UpdateAsync(qrCodeTarget, cancellationToken);

        return new Response { Id = command.Id };
    }
}