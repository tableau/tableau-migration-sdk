using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Content.Files
{
    /// <summary>
    /// Interface describing a logical file that is part of a content item.
    /// </summary>
    public interface IContentFileHandle : IAsyncDisposable
    {
        /// <summary>
        /// Gets the original filename of the file, used for the upload filename when publishing the content item.
        /// </summary>
        string OriginalFileName { get; }

        /// <summary>
        /// Gets the path to the file.
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Gets the file store the handle is for.
        /// </summary>
        IContentFileStore Store { get; }

        /// <summary>
        /// Opens a stream to read from a file.
        /// </summary>
        /// <param name="cancel">A cancellation token to obey.</param>
        /// <returns>The stream to read from.</returns>
        Task<IContentFileStream> OpenReadAsync(CancellationToken cancel);

        /// <summary>
        /// Opens a stream to write to a file.
        /// </summary>
        /// <param name="cancel">A cancellation token to obey.</param>
        /// <returns>The stream to write to.</returns>
        Task<IContentFileStream> OpenWriteAsync(CancellationToken cancel);

        /// <summary>
        /// Gets the current read/write stream to the XML content of the Tableau file, 
        /// opening a new stream if necessary.
        /// Changes to the stream will be automatically saved before publishing.
        /// </summary>
        /// <returns>The XML stream to edit.</returns>
        Task<ITableauFileXmlStream> GetXmlStreamAsync(CancellationToken cancel);
    }
}