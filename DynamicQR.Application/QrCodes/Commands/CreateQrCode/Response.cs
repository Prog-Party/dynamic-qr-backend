namespace DynamicQR.Application.QrCodes.Commands.CreateQrCode;

public sealed record Response
{
    public string Id { get; init; } = string.Empty;
}