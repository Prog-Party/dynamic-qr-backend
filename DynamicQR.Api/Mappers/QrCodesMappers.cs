using System.Drawing;

namespace DynamicQR.Api.Mappers;

public static class QrCodesMappers
{
    public static Application.QrCodes.Commands.CreateQrCode.Command? ToCore(this Endpoints.QrCodes.QrCodePost.Request request, string organizationId)
    {
        return request is null ? null : new Application.QrCodes.Commands.CreateQrCode.Command
        {
            BackgroundColor = ColorTranslator.FromHtml(request.BackgroundColor),
            ForegroundColor = ColorTranslator.FromHtml(request.ForegroundColor),
            ImageHeight = request.ImageHeight,
            ImageUrl = request.ImageUrl,
            ImageWidth = request.ImageWidth,
            IncludeMargin = request.IncludeMargin,
            Value = request.Value,
            OrganisationId = organizationId
        };
    }

    public static Endpoints.QrCodes.QrCodePost.Response? ToContract(this Application.QrCodes.Commands.CreateQrCode.Response response)
    {
        return response is null ? null : new Endpoints.QrCodes.QrCodePost.Response
        {
            Id = response.Id,
        };
    }

    public static Endpoints.QrCodes.QrCodeGet.Response? ToContract(this Application.QrCodes.Queries.GetQrCode.Response response)
    {
        return response is null ? null : new Endpoints.QrCodes.QrCodeGet.Response
        {
            BackgroundColor = ColorTranslator.ToHtml(response.BackgroundColor),
            ForegroundColor = ColorTranslator.ToHtml(response.ForegroundColor),
            ImageHeight = response.ImageHeight.GetValueOrDefault(),
            ImageUrl = response.ImageUrl,
            ImageWidth = response.ImageWidth.GetValueOrDefault(),
            IncludeMargin = response.IncludeMargin,
        };
    }

    public static Endpoints.QrCodes.QrCodeGetAll.Response? ToContract(this Application.QrCodes.Queries.GetAllQrCodes.Response response)
        => response is null ? null : new Endpoints.QrCodes.QrCodeGetAll.Response
        {
            Id = response.Id,
            BackgroundColor = ColorTranslator.ToHtml(response.BackgroundColor),
            ForegroundColor = ColorTranslator.ToHtml(response.ForegroundColor),
            ImageHeight = response.ImageHeight.GetValueOrDefault(),
            ImageUrl = response.ImageUrl,
            ImageWidth = response.ImageWidth.GetValueOrDefault(),
            IncludeMargin = response.IncludeMargin,
        };


    public static Endpoints.QrCodes.QrCodePut.Response? ToContract(this Application.QrCodes.Commands.UpdateQrCode.Response response)
    {
        return response is null ? null : new Endpoints.QrCodes.QrCodePut.Response
        {
            Id = response.Id,
        };
    }

    public static Application.QrCodes.Commands.UpdateQrCode.Command? ToCore(this Endpoints.QrCodes.QrCodePut.Request request, string id, string organizationId)
    {
        return request is null ? null : new Application.QrCodes.Commands.UpdateQrCode.Command
        {
            BackgroundColor = ColorTranslator.FromHtml(request.BackgroundColor),
            ForegroundColor = ColorTranslator.FromHtml(request.ForegroundColor),
            ImageHeight = request.ImageHeight,
            ImageUrl = request.ImageUrl,
            ImageWidth = request.ImageWidth,
            IncludeMargin = request.IncludeMargin,
            Id = id,
            OrganisationId = organizationId
        };
    }
}