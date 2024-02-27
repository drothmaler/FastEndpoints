using Microsoft.Net.Http.Headers;

namespace TestCases.TypedHeaderBindingTest;

sealed class MyRequest : PlainTextRequest
{
    [FromHeader("Content-Disposition")]
    public ContentDispositionHeaderValue Disposition { get; set; }
        
    [FromHeader("If-Match", isRequired:false)]
    public IList<EntityTagHeaderValue>? IfMatch { get; set; }
        
    [FromHeader("Accept")]
    public IList<MediaTypeHeaderValue> Accept { get; set; }
    
    [FromHeader("Accept-Encoding", isRequired:false)]
    public IList<StringWithQualityHeaderValue>? AcceptEncoding { get; set; }
}
sealed class MyResponse : PlainTextRequest
{
    public string FileName { get; set; }
    public IList<string> Accept { get; set; }
    public IList<string> AcceptEncoding { get; set; }

    [ToHeader("ETag")]
    public EntityTagHeaderValue? ETag { get; set; }

    [ToHeader("Set-Cookie")]
    public IList<SetCookieHeaderValue>? SetCookie { get; set; }
}

sealed class MyEndpoint : Endpoint<MyRequest, MyResponse>
{
    public override void Configure()
    {
        Post("test-cases/typed-header-binding-test");
        AllowAnonymous();
    }

    public override async Task HandleAsync(MyRequest r, CancellationToken c)
    {
        await SendAsync(new()
            {
                FileName = r.Disposition.FileName.Value!,
                Accept = r.Accept.Select(it => it.ToString()).ToList(),
                AcceptEncoding = (r.AcceptEncoding ?? Array.Empty<StringWithQualityHeaderValue>()).Select(it => it.ToString()).ToList(),
                ETag = new("\"33a64df551425fcc55e4d42a148795d9f25f89d4\""),
                SetCookie = new List<SetCookieHeaderValue> { new("foo", "bar"), new("bazz") } 
            });
    }
}