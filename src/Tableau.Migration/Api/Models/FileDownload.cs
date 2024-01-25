using System;
using System.IO;
using System.Threading.Tasks;

namespace Tableau.Migration.Api.Models
{
    /// <summary>
    /// Class representing a file download response.
    /// See Tableu API Reference for <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref.htm#download_data_source">data sources</see> and
    /// <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref.htm#download_workbook">workbooks</see> documentation.
    /// </summary>
    /// <param name="Filename">The server provided filename of the file to download.</param>
    /// <param name="Content">The stream with the file content to download from.</param>
    public record FileDownload(string? Filename, Stream Content) : IAsyncDisposable
    {
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous dispose operation.</returns>
        public async ValueTask DisposeAsync()
        {
            // Perform async cleanup.
            await Content.DisposeAsync().ConfigureAwait(false);

            // Suppress finalization.
            GC.SuppressFinalize(this);
        }
    }
}
