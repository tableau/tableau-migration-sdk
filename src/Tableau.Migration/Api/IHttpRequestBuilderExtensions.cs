using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Net;

namespace Tableau.Migration.Api
{
    /// <summary>
    /// Static class containing extension methods for <see cref="IHttpRequestBuilder"/> objects.
    /// </summary>
    internal static class IHttpRequestBuilderExtensions
    {
        /// <summary>
        /// Sends the request and treats the response as a file download.
        /// </summary>
        public static async Task<IAsyncDisposableResult<FileDownload>> DownloadAsync(this IHttpRequestBuilder requestBuilder, CancellationToken cancel)
        {
            //Send with ResponseHeadersRead completion option
            //so we do not internally buffer the file stream.
            var result = await requestBuilder
                .SendAsync(HttpCompletionOption.ResponseHeadersRead, cancel)
                .DownloadResultAsync(cancel)
                .ConfigureAwait(false);

            return result;
        }
    }
}
