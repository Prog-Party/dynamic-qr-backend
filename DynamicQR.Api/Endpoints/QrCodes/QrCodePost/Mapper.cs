using System.Drawing;
using ApplicationCommand = DynamicQR.Application.QrCodes.Commands.CreateQrCode.Command;
using ApplicationResponse = DynamicQR.Application.QrCodes.Commands.CreateQrCode.Response;

namespace DynamicQR.Api.Endpoints.QrCodes.QrCodePost;

public static class Mapper
{
    public static ApplicationCommand? ToCore(QrCodePostRequest request, string organizationId, string customerId)
        => request is null ? null : new ApplicationCommand
        {
            BackgroundColor = ColorTranslator.FromHtml(request.BackgroundColor),
            ForegroundColor = ColorTranslator.FromHtml(request.ForegroundColor),
            ImageHeight = request.ImageHeight,
            ImageUrl = request.ImageUrl,
            ImageWidth = request.ImageWidth,
            IncludeMargin = request.IncludeMargin,
            Value = request.Value,
            OrganizationId = organizationId,
            CustomerId = customerId
        };

    public static QrCodePostResponse? ToContract(ApplicationResponse response)
        => response is null ? null : new QrCodePostResponse
        {
            Id = response.Id,
        };
}
