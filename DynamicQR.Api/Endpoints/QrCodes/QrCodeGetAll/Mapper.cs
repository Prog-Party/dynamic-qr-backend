using System.Drawing;
using ApplicationResponse = DynamicQR.Application.QrCodes.Queries.GetAllQrCodes.Response;

namespace DynamicQR.Api.Endpoints.QrCodes.QrCodeGetAll;

public static class Mapper
{
    public static QrCodeGetAllResponse? ToContract(ApplicationResponse response)
        => response is null ? null : new QrCodeGetAllResponse
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
