using DynamicQR.Api.Attributes;
using DynamicQR.Api.Mappers;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Storage;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Net;

namespace DynamicQR.Api.Endpoints.QrCodeTargets.QrCodeTargetPut;

public sealed class QrCodeTargetPut : EndpointsBase
{
    public QrCodeTargetPut(IMediator mediator, ILoggerFactory loggerFactory) :
        base(mediator, loggerFactory.CreateLogger<QrCodeTargetPut>())
    { }

    [Function(nameof(QrCodeTargetPut))]
    [OpenApiOperation(nameof(QrCodeTargetPut), Tags.QrCodeTarget,
       Summary = "Update a certain qr code target.")
    ]
    [OpenApiPathIdentifier]
    [OpenApiJsonPayload(typeof(Request))]
    [OpenApiJsonResponse(typeof(Response), Description = "Update a certain qr code target")]
    [OpenApiResponseWithoutBody(HttpStatusCode.BadGateway, Description = "No qr code target found with the given identifier.")]
    public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Function, "put", Route = "qr-code-targets/{id}")] HttpRequestData req,
        string id,
        CancellationToken cancellationToken)
    {
        var request = await ParseBody<Request>(req);
        if (request.Error != null) return request.Error;

        Application.QrCodes.Commands.UpdateQrCodeTarget.Command? coreCommand = request.Result.ToCore(id);

        Application.QrCodes.Commands.UpdateQrCodeTarget.Response coreResponse;

        try
        {
            coreResponse = await _mediator.Send(coreCommand!, cancellationToken);
        }
        catch (StorageException)
        {
            return req.CreateResponse(HttpStatusCode.BadGateway);
        }

        Response? responseContent = coreResponse.ToContract();

        return await CreateJsonResponse(req, responseContent);
    }
}