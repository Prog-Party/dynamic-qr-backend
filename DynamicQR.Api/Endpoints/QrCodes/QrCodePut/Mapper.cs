﻿using System.Drawing;
using ApplicationCommand = DynamicQR.Application.QrCodes.Commands.UpdateQrCode.Command;
using ApplicationResponse = DynamicQR.Application.QrCodes.Commands.UpdateQrCode.Response;

namespace DynamicQR.Api.Endpoints.QrCodes.QrCodePut;

internal static class Mapper
{
    internal static ApplicationCommand? ToCore(this QrCodePutRequest request, string id, string organizationId, string customerId)
        => request is null ? null : new ApplicationCommand
        {
            BackgroundColor = ColorTranslator.FromHtml(request.BackgroundColor),
            ForegroundColor = ColorTranslator.FromHtml(request.ForegroundColor),
            ImageHeight = request.ImageHeight,
            ImageUrl = request.ImageUrl,
            ImageWidth = request.ImageWidth,
            IncludeMargin = request.IncludeMargin,
            Id = id,
            OrganizationId = organizationId,
            CustomerId = customerId
        };

    internal static QrCodePutResponse? ToContract(this ApplicationResponse response)
        => response is null ? null : new QrCodePutResponse
        {
            Id = response.Id,
        };
}
