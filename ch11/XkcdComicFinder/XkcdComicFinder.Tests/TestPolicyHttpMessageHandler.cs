using Microsoft.Extensions.Http;
using Polly;

namespace XkcdComicFinder.Tests;

internal class TestPolicyHttpMessageHandler 
  : PolicyHttpMessageHandler
{
  public Func<HttpRequestMessage, Context, 
    CancellationToken, Task<HttpResponseMessage>> 
    OnSendAsync { get; set; } = null!;

  public TestPolicyHttpMessageHandler(
    IAsyncPolicy<HttpResponseMessage> policy)
    : base(policy) { }

  public TestPolicyHttpMessageHandler(
    Func<HttpRequestMessage, 
    IAsyncPolicy<HttpResponseMessage>> policySelector)
    : base(policySelector) { }

  protected override Task<HttpResponseMessage> 
    SendCoreAsync(
    HttpRequestMessage request, 
    Context context, 
    CancellationToken cancellationToken)
    => OnSendAsync(request, context, cancellationToken);
}