using DynamicQR.Api.Attributes;
using DynamicQR.Api.Extensions;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Storage;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Net;
using ApplicationCommand = DynamicQR.Application.QrCodes.Commands.UpdateQrCode.Command;
using ApplicationResponse = DynamicQR.Application.QrCodes.Commands.UpdateQrCode.Response;

namespace DynamicQR.Api.Endpoints.QrCodes.QrCodePatch;

public sealed class QrCodePatch : EndpointsBase
{
    public QrCodePatch(IMediator mediator, ILoggerFactory loggerFactory) :
        base(mediator, loggerFactory.CreateLogger<QrCodePatch>())
    {
    }

    [Function(nameof(QrCodePatch))]
    [OpenApiOperation(nameof(QrCodePatch), Tags.QrCode,
       Summary = "Update a certain qr code.")
    ]
    [OpenApiHeaderOrganizationIdentifier]
    [OpenApiHeaderCustomerIdentifier]
    [OpenApiPathIdentifier]
    [OpenApiJsonPayload(typeof(QrCodePatchRequest))]
    [OpenApiJsonResponse(typeof(QrCodePatchResponse), Description = "Update a certain qr code")]
    [OpenApiResponseWithoutBody(HttpStatusCode.BadRequest, Description = "No qr code found with the given identifier. Or missing organization identifier header. Or missing customer identifier header.")]
    public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Function, "put", Route = "qr-codes/{id}")] HttpRequestData req,
        string id,
        CancellationToken cancellationToken)
    {
        string organizationId = req.GetHeaderAttribute<OpenApiHeaderOrganizationIdentifierAttribute>();
        string customerId = req.GetHeaderAttribute<OpenApiHeaderCustomerIdentifierAttribute>();

        var request = await ParseBody<QrCodePatchRequest>(req);
        if (request.Error != null) return request.Error;

        ApplicationCommand? coreCommand = request.Result.ToCore(id, organizationId, customerId);

        ApplicationResponse coreResponse;

        try
        {
            coreResponse = await _mediator.Send(coreCommand!, cancellationToken);
        }
        catch (StorageException)
        {
            return req.CreateResponse(HttpStatusCode.BadGateway);
        }

        QrCodePatchResponse? responseContent = coreResponse.ToContract();

        return await CreateJsonResponse(req, responseContent);
    }
}