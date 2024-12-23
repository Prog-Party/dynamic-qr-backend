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
    private readonly IQrCodeHistoryRepositoryService _qrCodeHistoryRepositoryService;
    private const int QrCodeIdLength = 8;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="qrCodeRepositoryService"></param>
    /// <param name="qrCodeTargetRepositoryService"></param>
    /// <param name="qrCodeHistoryRepositoryService"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public CommandHandler(IQrCodeRepositoryService qrCodeRepositoryService, IQrCodeTargetRepositoryService qrCodeTargetRepositoryService, IQrCodeHistoryRepositoryService qrCodeHistoryRepositoryService)
    {
        _qrCodeRepositoryService = qrCodeRepositoryService ?? throw new ArgumentNullException(nameof(qrCodeRepositoryService));
        _qrCodeTargetRepositoryService = qrCodeTargetRepositoryService ?? throw new ArgumentNullException(nameof(qrCodeTargetRepositoryService));
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

        string id;
        bool isUnique;
        do
        {
            id = GenerateQrCodeId();
            isUnique = await IsQrCodeIdUnique(id, command.OrganizationId, cancellationToken);
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

        await LogHistory(qrCode.Id, command, cancellationToken);
        await _qrCodeRepositoryService.CreateAsync(command.OrganizationId, qrCode, cancellationToken);
        await _qrCodeTargetRepositoryService.CreateAsync(qrCodeTarget, cancellationToken);

        return new Response
        {
            Id = qrCode.Id
        };
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


    private async Task<bool> IsQrCodeIdUnique(string id, string organisationId, CancellationToken cancellationToken)
        => !await _qrCodeTargetRepositoryService.Exists(id, cancellationToken);

    private async Task LogHistory(string id, Command command, CancellationToken cancellationToken)
    {
        QrCodeHistory historyItem = new()
        {
            QrCodeId = id,
            CustomerId = command.CustomerId,
            OrganizationId = command.OrganizationId,
            EventType = QrCodeEvents.Lifecycle.Created
        };

        await _qrCodeHistoryRepositoryService.AddHistoryAsync(historyItem, cancellationToken);
    }
}
