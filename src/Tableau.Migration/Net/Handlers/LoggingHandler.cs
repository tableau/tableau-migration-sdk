using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Net.Handlers
{
    internal class LoggingHandler : DelegatingHandler
    {
        private readonly INetworkTraceLogger _traceLogger;

        public LoggingHandler(INetworkTraceLogger traceLogger)
        {
            _traceLogger = traceLogger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            try
            {
                var response = await base
                    .SendAsync(request, cancellationToken)
                    .ConfigureAwait(false);

                await _traceLogger
                    .WriteNetworkLogsAsync(
                        request,
                        response,
                        cancellationToken)
                    .ConfigureAwait(false);

                return response;
            }
            catch (Exception ex)
            {
                await _traceLogger
                    .WriteNetworkExceptionLogsAsync(
                        request,
                        ex,
                        cancellationToken)
                    .ConfigureAwait(false);

                throw;
            }
        }
    }
}
