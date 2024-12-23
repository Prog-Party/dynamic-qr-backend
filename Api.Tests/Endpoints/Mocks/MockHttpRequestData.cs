using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;

namespace Api.Tests.Endpoints.Mocks;

public class MockHttpRequestData : HttpRequestData
{
    private readonly MemoryStream _bodyStream;
    private readonly HttpMethod _method;

    public MockHttpRequestData(FunctionContext context, HttpMethod method, Dictionary<string, string>? headers = null, object body = null!)
        : base(context)
    {
        Headers = new HttpHeadersCollection();

        if (headers != null)
        {
            foreach (var header in headers)
            {
                Headers.Add(header.Key, header.Value);
            }
        }

        var jsonBody = JsonConvert.SerializeObject(body);
        _bodyStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonBody));
        _method = method;
    }

    public override HttpHeadersCollection Headers { get; }
    public override Stream Body => _bodyStream;
    public override Uri Url => new Uri("https://localhost");
    public override IEnumerable<ClaimsIdentity> Identities => new List<ClaimsIdentity>();
    public override string Method => _method.Method;

    public override IReadOnlyCollection<IHttpCookie> Cookies => throw new NotImplementedException();

    public override HttpResponseData CreateResponse()
    {
        return new MockHttpResponseData(FunctionContext);
    }
}