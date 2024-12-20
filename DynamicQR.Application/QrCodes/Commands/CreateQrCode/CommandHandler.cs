using DynamicQR.Domain.Interfaces;
using DynamicQR.Domain.Models;
using MediatR;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("Application.Tests")]
namespace DynamicQR.Application.QrCodes.Commands.CreateQrCode;

public class CommandHandler : IRequestHandler<Command, Response>
{
    private readonly IQrCodeRepositoryService _qrCodeRepositoryService;
    private readonly IQrCodeTargetRepositoryService _qrCodeTargetRepositoryService;
    private const int QrCodeIdLength = 8;

    public CommandHandler(IQrCodeRepositoryService qrCodeRepositoryService, IQrCodeTargetRepositoryService qrCodeTargetRepositoryService)
    {
        _qrCodeRepositoryService = qrCodeRepositoryService ?? throw new ArgumentNullException(nameof(qrCodeRepositoryService));
        _qrCodeTargetRepositoryService = qrCodeTargetRepositoryService ?? throw new ArgumentNullException(nameof(qrCodeTargetRepositoryService));
    }

    internal string GenerateQrCodeId()
    {
        const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
        Random random = new();
        var result = new StringBuilder(QrCodeIdLength);
        for (int i = 0; i < QrCodeIdLength; i++)
        {
            result.Append(chars[random.Next(chars.Length)]);
        }
        return result.ToString();
    }

    public async Task<Response> Handle(Command command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        string id;
        bool isUnique;
        do
        {
            id = GenerateQrCodeId();
            isUnique = await IsQrCodeIdUnique(id, command.OrganisationId, cancellationToken);
        } while (!isUnique);

        QrCode qrCode = new()
        {
            BackgroundColor = command.BackgroundColor,
            ForegroundColor = command.ForegroundColor,
            Id = id,
            ImageHeight = command.ImageHeight,
            ImageWidth = command.ImageWidth,
            ImageUrl = command.ImageUrl,
            IncludeMargin = command.IncludeMargin,
        };

        QrCodeTarget qrCodeTarget = new()
        {
            QrCodeId = id,
            Value = command.Value
        };

        await _qrCodeRepositoryService.CreateAsync(command.OrganisationId, qrCode, cancellationToken);
        await _qrCodeTargetRepositoryService.CreateAsync(qrCodeTarget, cancellationToken);

        return new Response
        {
            Id = qrCode.Id
        };
    }

    private async Task<bool> IsQrCodeIdUnique(string id, string organisationId, CancellationToken cancellationToken)
        => !await _qrCodeTargetRepositoryService.Exists(id, cancellationToken);
}
