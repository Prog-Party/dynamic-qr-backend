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
using DynamicQR.Domain.Interfaces;
using DynamicQR.Domain.Models;
using DynamicQR.Domain;

namespace DynamicQR.Api.Endpoints.QrCodes.QrCodePut;

public sealed class QrCodePut : EndpointsBase
{
    private readonly IQrCodeHistoryRepositoryService _qrCodeHistoryRepositoryService;

    public QrCodePut(IMediator mediator, ILoggerFactory loggerFactory, IQrCodeHistoryRepositoryService qrCodeHistoryRepositoryService) :
        base(mediator, loggerFactory.CreateLogger<QrCodePut>())
    {
        _qrCodeHistoryRepositoryService = qrCodeHistoryRepositoryService ?? throw new ArgumentNullException(nameof(qrCodeHistoryRepositoryService));
    }

    [Function(nameof(QrCodePut))]
    [OpenApiOperation(nameof(QrCodePut), Tags.QrCode,
       Summary = "Update a certain qr code.")
    ]
    [OpenApiHeaderOrganizationIdentifier]
    [OpenApiHeaderCustomerIdentifier]
    [OpenApiPathIdentifier]
    [OpenApiJsonPayload(typeof(Request))]
    [OpenApiJsonResponse(typeof(Response), Description = "Update a certain qr code")]
    [OpenApiResponseWithoutBody(HttpStatusCode.BadRequest, Description = "No qr code found with the given identifier. Or Missing organization identifier header")]
    public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Function, "put", Route = "qr-codes/{id}")] HttpRequestData req,
        string id,
        CancellationToken cancellationToken)
    {
        string organizationId = req.GetHeaderAttribute<OpenApiHeaderOrganizationIdentifierAttribute>();
        string customerId = req.GetHeaderAttribute<OpenApiHeaderCustomerIdentifierAttribute>();

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

        await LogHistory(id, organizationId, customerId, cancellationToken);

        return await CreateJsonResponse(req, responseContent);
    }

    private async Task LogHistory(string qrCodeId, string organizationId, string customerId, CancellationToken cancellationToken)
    {
        QrCodeHistory historyItem = new()
        {
            CustomerId = customerId,
            OrganizationId = organizationId,
            EventType = QrCodeEvents.Lifecycle.Updated
        };

        await _qrCodeHistoryRepositoryService.AddHistoryAsync(historyItem, cancellationToken);
    }
}
