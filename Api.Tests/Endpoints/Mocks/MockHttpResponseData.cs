using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.Json;

namespace Api.Tests.Endpoints.Mocks;

[ExcludeFromCodeCoverage]
public class MockHttpResponseData : HttpResponseData
{
    private readonly StreamWriter _writer;

    public MockHttpResponseData(FunctionContext functionContext, HttpStatusCode statusCode = HttpStatusCode.OK) : base(functionContext)
    {
        StatusCode = statusCode;
        _writer = new StreamWriter(Body) { AutoFlush = true };
    }

    public override HttpStatusCode StatusCode { get; set; }
    public override HttpHeadersCollection Headers { get; set; } = new HttpHeadersCollection();
    public override Stream Body { get; set; } = new MemoryStream();
    public override HttpCookies Cookies { get; }

    public async Task WriteStringAsync(string text)
    {
        await _writer.WriteAsync(text);
        Body.Seek(0, SeekOrigin.Begin);
    }

    public async Task<string> ReadAsStringAsync()
    {
        Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(Body);
        return await reader.ReadToEndAsync();
    }

    public async Task<T?> ReadAsJsonAsync<T>()
    {
        var content = await ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true // Handles case-insensitive property matching
        });
    }
}