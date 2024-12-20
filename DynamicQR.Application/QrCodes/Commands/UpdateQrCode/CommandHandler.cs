using DynamicQR.Domain.Interfaces;
using DynamicQR.Domain.Models;
using MediatR;

namespace DynamicQR.Application.QrCodes.Commands.UpdateQrCode;

public class CommandHandler : IRequestHandler<Command, Response>
{
    private readonly IQrCodeRepositoryService _qrCodeRepositoryService;

    public CommandHandler(IQrCodeRepositoryService qrCodeRepositoryService)
    {
        _qrCodeRepositoryService = qrCodeRepositoryService ?? throw new ArgumentNullException(nameof(qrCodeRepositoryService));
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

        await _qrCodeRepositoryService.UpdateAsync(command.OrganisationId, qrCode, cancellationToken);

        return new Response { Id = command.Id };
    }
}