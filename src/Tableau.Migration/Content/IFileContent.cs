using System;
using Tableau.Migration.Content.Files;

namespace Tableau.Migration.Content
{
    /// <summary>
    /// Interface for content items that publish via file upload.
    /// </summary>
    public interface IFileContent : IAsyncDisposable
    {
        /// <summary>
        /// Gets or sets the file to publish for the content item.
        /// </summary>
        IContentFileHandle File { get; set; }
    }
}