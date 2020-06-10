using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.JRSClient.Extensions
{
    public class JasperCookieHandler : HttpClientHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
