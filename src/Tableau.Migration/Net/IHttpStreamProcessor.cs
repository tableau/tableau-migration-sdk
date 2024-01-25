using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Net
{
    /// <summary>
    /// Interface for classes that process <see cref="Stream"/>s for HTTP requests and responses.
    /// </summary>
    public interface IHttpStreamProcessor
    {
        /// <summary>
        /// Processes the stream in chunks and sends the created requests.
        /// </summary>
        /// <typeparam name="TResponse">The response type.</typeparam>
        /// <param name="stream">The stream to process.</param>
        /// <param name="buildChunkRequest">
        /// Function to build an HTTP request from a chunk of data from the stream.
        /// The first parameter is the chunk of data, or possibly a partial chunk of data.
        /// The second parameter is the count of bytes of the chunk of data to send.</param>
        /// <param name="cancel">A cancellation token to obey.</param>
        /// <returns>A collection of responses from the chunked requests.</returns>
        Task<IEnumerable<IHttpResponseMessage<TResponse>>> ProcessAsync<TResponse>(
            Stream stream,
            Func<byte[], int, HttpRequestMessage> buildChunkRequest,
            CancellationToken cancel) where TResponse : class;
    }
}