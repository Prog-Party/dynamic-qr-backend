using System.Drawing;
using ApplicationResponse = DynamicQR.Application.QrCodes.Queries.GetQrCode.Response;

namespace DynamicQR.Api.Endpoints.QrCodes.QrCodeGet;

internal static class Mapper
{
    internal static QrCodeGetResponse? ToContract(ApplicationResponse response)
        => response is null ? null : new QrCodeGetResponse
        {
            Value = response.Value,
            BackgroundColor = ColorTranslator.ToHtml(response.BackgroundColor),
            ForegroundColor = ColorTranslator.ToHtml(response.ForegroundColor),
            ImageHeight = response.ImageHeight.GetValueOrDefault(),
            ImageUrl = response.ImageUrl,
            ImageWidth = response.ImageWidth.GetValueOrDefault(),
            IncludeMargin = response.IncludeMargin,
        };
}
