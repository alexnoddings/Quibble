using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace Quibble.Client.Handlers
{
    /// <summary>
    /// Includes credentials in HTTP requests to the same origin.
    /// </summary>
    public class SameOriginRequestCredentialsHttpMessageHandler : DelegatingHandler
    {
        /// <summary>Creates a new instance of the <see cref="SameOriginRequestCredentialsHttpMessageHandler" /> class with a specific inner handler.</summary>
        /// <param name="innerHandler">The inner handler which is responsible for processing the HTTP response messages.</param>
        public SameOriginRequestCredentialsHttpMessageHandler(HttpMessageHandler innerHandler) : base(innerHandler)
        {
        }

        /// <summary>
        /// Configures the request to send credentials to same origin requests then delegates control to the inner handler.
        /// </summary>
        /// <param name="request">The HTTP request message being sent.</param>
        /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.SetBrowserRequestCredentials(BrowserRequestCredentials.SameOrigin);
            return base.SendAsync(request, cancellationToken);
        }
    }
}
