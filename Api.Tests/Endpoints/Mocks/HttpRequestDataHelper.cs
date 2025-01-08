using Azure.Core.Serialization;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using System.Text;

namespace Api.Tests.Endpoints.Mocks;

// Helper class for creating HttpRequestData for testing
internal class HttpRequestDataHelper
{
    public static MockHttpRequestData CreateWithHeaders(HttpMethod method, Dictionary<string, string>? headers = null)
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddOptions<WorkerOptions>().Configure(options =>
        {
            options.Serializer = new NewtonsoftJsonObjectSerializer();
        });

        var functionContextMock = new Mock<FunctionContext>();
        functionContextMock.Setup(f => f.InstanceServices)
            .Returns(serviceCollection.BuildServiceProvider());

        var req = new MockHttpRequestData(functionContextMock.Object, method, headers);

        return req;
    }

    public static HttpRequestData CreateWithJsonBody(HttpMethod method, Dictionary<string, string> headers, object body)
    {
        var jsonBody = JsonConvert.SerializeObject(body);
        Stream Body = new MemoryStream(Encoding.UTF8.GetBytes(jsonBody));

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddOptions<WorkerOptions>().Configure(options =>
        {
            options.Serializer = new NewtonsoftJsonObjectSerializer();
        });

        var functionContextMock = new Mock<FunctionContext>();
        functionContextMock.Setup(f => f.InstanceServices)
            .Returns(serviceCollection.BuildServiceProvider());

        var req = new MockHttpRequestData(functionContextMock.Object, method, headers, body);

        return req;
    }
}