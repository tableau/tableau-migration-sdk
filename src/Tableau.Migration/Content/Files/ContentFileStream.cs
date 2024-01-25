using System;
using System.IO;
using System.Threading.Tasks;

namespace Tableau.Migration.Content.Files
{
    /// <summary>
    /// Default <see cref="IContentFileStream"/> implementation
    /// that owns and disposes of a content file stream.
    /// </summary>
    public class ContentFileStream : IContentFileStream
    {
        /// <inheritdoc />
        public Stream Content { get; }

        /// <summary>
        /// Creates a new <see cref="ContentFileStream"/> object.
        /// </summary>
        /// <param name="content">The content stream to take ownership of.</param>
        public ContentFileStream(Stream content)
        {
            Content = content;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous dispose operation.</returns>
        public virtual async ValueTask DisposeAsync()
        {
            // Perform async cleanup.
            await Content.DisposeAsync().ConfigureAwait(false);

            // Suppress finalization.
            GC.SuppressFinalize(this);
        }
    }
}
