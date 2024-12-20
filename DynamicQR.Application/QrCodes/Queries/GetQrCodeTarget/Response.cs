namespace DynamicQR.Application.QrCodes.Queries.GetQrCodeTarget;

public sealed record Response
{
    public string QrCodeId { get; init; } = string.Empty;
    public string Value { get; init; } = string.Empty;
}