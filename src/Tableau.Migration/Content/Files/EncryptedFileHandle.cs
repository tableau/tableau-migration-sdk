using System;
using System.Threading.Tasks;

namespace Tableau.Migration.Content.Files
{
    /// <summary>
    /// <see cref="ContentFileHandle"/> that is potentially stored in encrypted form.
    /// </summary>
    /// <param name="Store"><inheritdoc /></param>
    /// <param name="Path"><inheritdoc /></param>
    /// <param name="OriginalFileName"><inheritdoc /></param>
    /// <param name="Inner">The file handle to the inner file store.</param>
    public record EncryptedFileHandle(IContentFileStore Store, string Path, string OriginalFileName, IContentFileHandle Inner)
        : ContentFileHandle(Store, Path, OriginalFileName)
    {
        /// <summary>
        /// Creates a new <see cref="EncryptedFileHandle"/> object.
        /// </summary>
        /// <param name="store">The file store the handle is for.</param>
        /// <param name="inner">The file handle to the inner file store.</param>
        public EncryptedFileHandle(IContentFileStore store, IContentFileHandle inner)
            : this(store, inner.Path, inner.OriginalFileName, inner)
        { }

        /// <inheritdoc />
        public override async ValueTask DisposeAsync()
        {
            // Perform async cleanup.
            await Inner.DisposeAsync().ConfigureAwait(false);

            // Suppress finalization.
            GC.SuppressFinalize(this);
        }
    }
}
