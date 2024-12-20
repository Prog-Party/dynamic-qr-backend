using DynamicQR.Api.Attributes;
using DynamicQR.Api.Extensions;
using DynamicQR.Api.Mappers;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Storage;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Net;

namespace DynamicQR.Api.Endpoints.QrCodes.QrCodePut;

public sealed class QrCodePut : EndpointsBase
{
    public QrCodePut(IMediator mediator, ILoggerFactory loggerFactory) :
        base(mediator, loggerFactory.CreateLogger<QrCodePut>())
    { }

    [Function(nameof(QrCodePut))]
    [OpenApiOperation(nameof(QrCodePut), Tags.QrCode,
       Summary = "Update a certain qr code.")
    ]
    [OpenApiHeaderOrganizationIdentifier]
    [OpenApiPathIdentifier]
    [OpenApiJsonPayload(typeof(Request))]
    [OpenApiJsonResponse(typeof(Response), Description = "Update a certain qr code")]
    [OpenApiResponseWithoutBody(HttpStatusCode.BadRequest, Description = "No qr code found with the given identifier. Or Missing organization identifier header")]
    public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Function, "put", Route = "qr-codes/{id}")] HttpRequestData req,
        string id,
        CancellationToken cancellationToken)
    {
        string organizationId = req.GetHeaderAttribute<OpenApiHeaderOrganizationIdentifierAttribute>();

        var request = await ParseBody<Request>(req);
        if (request.Error != null) return request.Error;

        Application.QrCodes.Commands.UpdateQrCode.Command? coreCommand = QrCodesMappers.ToCore(request.Result, id, organizationId);

        Application.QrCodes.Commands.UpdateQrCode.Response coreResponse;

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