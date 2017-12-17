using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Correlation.WebApiHost
{
    public class CorrelationContextMessageHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return base.SendAsync(request, cancellationToken);
        }
    }
}
