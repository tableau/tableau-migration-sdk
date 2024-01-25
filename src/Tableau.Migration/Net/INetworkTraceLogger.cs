using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Net
{
    internal interface INetworkTraceLogger
    {
        Task WriteNetworkLogsAsync(
            HttpRequestMessage request,
            HttpResponseMessage response,
            CancellationToken cancel);

        Task WriteNetworkExceptionLogsAsync(
            HttpRequestMessage request,
            Exception ex,
            CancellationToken cancel);
    }
}
