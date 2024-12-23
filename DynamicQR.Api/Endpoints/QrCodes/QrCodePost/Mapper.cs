using System.Drawing;
using ApplicationCommand = DynamicQR.Application.QrCodes.Commands.CreateQrCode.Command;
using ApplicationResponse = DynamicQR.Application.QrCodes.Commands.CreateQrCode.Response;

namespace DynamicQR.Api.Endpoints.QrCodes.QrCodePost;

internal static class Mapper
{
    internal static ApplicationCommand? ToCore(Request request, string organizationId, string customerId)
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

    internal static Response? ToContract(ApplicationResponse response)
        => response is null ? null : new Response
        {
            Id = response.Id,
        };
}
