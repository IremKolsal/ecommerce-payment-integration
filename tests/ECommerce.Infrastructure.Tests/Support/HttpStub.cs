using System.Net;
using System.Text;
using System.Text.Json;

internal static class HttpStub
{
    public static HttpClient FromJson(object payload, HttpStatusCode status = HttpStatusCode.OK)
    {
        var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var msg = new HttpResponseMessage(status)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        return new HttpClient(new StubHandler(msg)) { BaseAddress = new Uri("http://localhost/") };
    }

    public static HttpClient FromRaw(string raw, HttpStatusCode status = HttpStatusCode.OK)
    {
        var msg = new HttpResponseMessage(status)
        {
            Content = new StringContent(raw ?? "", Encoding.UTF8, "application/json")
        };
        return new HttpClient(new StubHandler(msg)) { BaseAddress = new Uri("http://localhost/") };
    }
}

internal sealed class StubHandler : HttpMessageHandler
{
    private readonly HttpResponseMessage _response;
    public StubHandler(HttpResponseMessage response) => _response = response;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage _, CancellationToken __)
        => Task.FromResult(_response);
}
