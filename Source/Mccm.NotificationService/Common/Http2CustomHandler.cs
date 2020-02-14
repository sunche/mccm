using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Mccm.NotificationService.Common
{
    /// <summary>
    /// Use to send to Apple servers.
    /// </summary>
    public class Http2CustomHandler : WinHttpHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Version = new Version("2.0");
            return base.SendAsync(request, cancellationToken);
        }
    }
}
