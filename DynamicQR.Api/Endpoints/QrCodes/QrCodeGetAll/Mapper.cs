using System.Drawing;
using ApplicationResponse = DynamicQR.Application.QrCodes.Queries.GetAllQrCodes.Response;

namespace DynamicQR.Api.Endpoints.QrCodes.QrCodeGetAll;

internal static class Mapper
{
    internal static QrCodeGetAllResponse ToContract(ApplicationResponse response)
        => response is null ? throw new ArgumentNullException(nameof(response)) : new QrCodeGetAllResponse
        {
            Id = response.Id,
            BackgroundColor = ColorTranslator.ToHtml(response.BackgroundColor),
            ForegroundColor = ColorTranslator.ToHtml(response.ForegroundColor),
            ImageHeight = response.ImageHeight.GetValueOrDefault(),
            ImageUrl = response.ImageUrl,
            ImageWidth = response.ImageWidth.GetValueOrDefault(),
            IncludeMargin = response.IncludeMargin,
        };
}
