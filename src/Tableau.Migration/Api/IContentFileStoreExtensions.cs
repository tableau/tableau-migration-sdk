using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Content.Files;

namespace Tableau.Migration.Api
{
    internal static class IContentFileStoreExtensions
    {
        /// <summary>
        /// Creates a file managed by the file store from a download stream.
        /// </summary>
        /// <param name="store">The file store to save to.</param>
        /// <param name="contentItem">The content item to resolve a relative file store path from.</param>
        /// <param name="download">The downloaded file.</param>
        /// <param name="cancel">A cancellation token to obey.</param>
        /// <returns>A handle to the newly created file.</returns>
        public static async Task<IContentFileHandle> CreateAsync<T>(this IContentFileStore store, T contentItem, FileDownload download, CancellationToken cancel)
            => await store.CreateAsync(contentItem, download.Filename ?? string.Empty, download.Content, cancel).ConfigureAwait(false);
    }
}
