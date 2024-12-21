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

namespace DynamicQR.Api.Endpoints.QrCodes.QrCodePost;

public sealed class QrCodePost : EndpointsBase
{
    private readonly IQrCodeHistoryRepositoryService _qrCodeHistoryRepositoryService;

    public QrCodePost(IMediator mediator, ILoggerFactory loggerFactory, IQrCodeHistoryRepositoryService qrCodeHistoryRepositoryService) :
        base(mediator, loggerFactory.CreateLogger<QrCodePost>())
    {
        _qrCodeHistoryRepositoryService = qrCodeHistoryRepositoryService ?? throw new ArgumentNullException(nameof(qrCodeHistoryRepositoryService));
    }

    [Function(nameof(QrCodePost))]
    [OpenApiOperation(nameof(QrCodePost), Tags.QrCode,
       Summary = "Create a new qr code.")
    ]
    [OpenApiHeaderOrganizationIdentifier]
    [OpenApiHeaderCustomerIdentifier]
    [OpenApiJsonPayload(typeof(Request))]
    [OpenApiJsonResponse(typeof(Response), HttpStatusCode.Created, Description = "Get a certain qr code")]
    [OpenApiResponseWithoutBody(HttpStatusCode.BadGateway)]
    [OpenApiResponseWithoutBody(HttpStatusCode.BadRequest, Description = "Missing organization identifier header")]
    public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Function, "post", Route = "qr-codes")] HttpRequestData req,
        CancellationToken cancellationToken)
    {
        string organizationId = req.GetHeaderAttribute<OpenApiHeaderOrganizationIdentifierAttribute>();
        string customerId = req.GetHeaderAttribute<OpenApiHeaderCustomerIdentifierAttribute>();

        var request = await ParseBody<Request>(req);
        if (request.Error != null) return request.Error;

        Application.QrCodes.Commands.CreateQrCode.Command? coreCommand = QrCodesMappers.ToCore(request.Result, organizationId);

        Application.QrCodes.Commands.CreateQrCode.Response coreResponse;

        try
        {
            coreResponse = await _mediator.Send(coreCommand!, cancellationToken);
        }
        catch (StorageException)
        {
            return req.CreateResponse(HttpStatusCode.BadGateway);
        }

        Response? responseContent = coreResponse.ToContract();

        await LogHistory(coreResponse.Id, organizationId, customerId, cancellationToken);

        return await CreateJsonResponse(req, responseContent, HttpStatusCode.Created);
    }

    private async Task LogHistory(string qrCodeId, string organizationId, string customerId, CancellationToken cancellationToken)
    {
        QrCodeHistory historyItem = new()
        {
            CustomerId = customerId,
            OrganizationId = organizationId,
            EventType = QrCodeEvents.Lifecycle.Created
        };

        await _qrCodeHistoryRepositoryService.AddHistoryAsync(historyItem, cancellationToken);
    }
}
