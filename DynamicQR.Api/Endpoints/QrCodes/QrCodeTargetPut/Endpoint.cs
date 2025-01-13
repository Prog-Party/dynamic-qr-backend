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
using ApplicationCommand = DynamicQR.Application.QrCodes.Commands.UpdateQrCodeTarget.Command;
using ApplicationResponse = DynamicQR.Application.QrCodes.Commands.UpdateQrCodeTarget.Response;

namespace DynamicQR.Api.Endpoints.QrCodes.QrCodeTargetPut;

public sealed class QrCodeTargetPut : EndpointsBase
{
    public QrCodeTargetPut(IMediator mediator, ILoggerFactory loggerFactory) :
        base(mediator, loggerFactory.CreateLogger<QrCodeTargetPut>())
    { }

    [Function(nameof(QrCodeTargetPut))]
    [OpenApiOperation(nameof(QrCodeTargetPut), Tags.QrCodeTarget,
       Summary = "Update a certain qr code target.")
    ]
    [OpenApiHeaderOrganizationIdentifier]
    [OpenApiHeaderCustomerIdentifier]
    [OpenApiPathIdentifier]
    [OpenApiJsonPayload(typeof(QrCodeTargetPutRequest))]
    [OpenApiJsonResponse(typeof(QrCodeTargetPutResponse), Description = "Update a certain qr code target")]
    [OpenApiResponseWithoutBody(HttpStatusCode.BadRequest, Description = "Request couldn't be parsed. Or missing organization identifier header. Or missing customer identifier header.")]
    [OpenApiResponseWithoutBody(HttpStatusCode.BadGateway, Description = "No qr code target found with the given identifier.")]
    public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Function, "put", Route = "qr-codes/{id}/target")] HttpRequestData req,
        string id,
        CancellationToken cancellationToken)
    {
        string organizationId = req.GetHeaderAttribute<OpenApiHeaderOrganizationIdentifierAttribute>();
        string customerId = req.GetHeaderAttribute<OpenApiHeaderCustomerIdentifierAttribute>();

        var request = await ParseBody<QrCodeTargetPutRequest>(req);
        if (request.Error != null) return request.Error;

        ApplicationCommand coreCommand = Mapper.ToCore(request.Result, id, organizationId, customerId);

        ApplicationResponse coreResponse;

        try
        {
            coreResponse = await _mediator.Send(coreCommand, cancellationToken);
        }
        catch (StorageException)
        {
            return req.CreateResponse(HttpStatusCode.BadGateway);
        }

        QrCodeTargetPutResponse? responseContent = Mapper.ToContract(coreResponse);

        return await CreateJsonResponse(req, responseContent);
    }
}
