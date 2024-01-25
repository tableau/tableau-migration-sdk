using System;
using System.IO;

namespace Tableau.Migration.Content.Files
{
    /// <summary>
    /// Interface for a content file stream,
    /// with any associated disposable objects.
    /// </summary>
    public interface IContentFileStream : IAsyncDisposable
    {
        /// <summary>
        /// Gets the stream to read file content from or write file content to.
        /// </summary>
        Stream Content { get; }
    }
}
