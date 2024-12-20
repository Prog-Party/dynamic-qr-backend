using DynamicQR.Domain.Interfaces;
using MediatR;

namespace DynamicQR.Application.QrCodes.Commands.DeleteQrCode;

public class CommandHandler : IRequestHandler<Command, Unit>
{
    private readonly IQrCodeRepositoryService _qrCodeRepositoryService;
    private readonly IQrCodeTargetRepositoryService _qrCodeTargetRepositoryService;

    public CommandHandler(IQrCodeRepositoryService qrCodeRepositoryService, IQrCodeTargetRepositoryService qrCodeTargetRepositoryService)
    {
        _qrCodeRepositoryService = qrCodeRepositoryService ?? throw new ArgumentNullException(nameof(qrCodeRepositoryService));
        _qrCodeTargetRepositoryService = qrCodeTargetRepositoryService ?? throw new ArgumentNullException(nameof(qrCodeTargetRepositoryService));
    }

    public async Task<Unit> Handle(Command command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        await _qrCodeTargetRepositoryService.DeleteAsync(command.Id, cancellationToken);
        await _qrCodeRepositoryService.DeleteAsync(command.OrganisationId, command.Id, cancellationToken);

        return Unit.Value;
    }
}