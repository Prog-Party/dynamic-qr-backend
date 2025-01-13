namespace DynamicQR.Api.Endpoints.QrCodes.QrCodePatch;

public sealed record QrCodePatchResponse
{
    public string Id { get; init; } = string.Empty;
}