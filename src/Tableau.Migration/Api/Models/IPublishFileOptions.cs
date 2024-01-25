using System.IO;

namespace Tableau.Migration.Api.Models
{
    /// <summary>
    /// Interface for API client file publish options. 
    /// </summary>
    public interface IPublishFileOptions
    {
        /// <summary>
        /// Get the file content stream.
        /// </summary>
        Stream File { get; }

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        string FileName { get; }

        /// <summary>
        /// Gets the type of the file.
        /// </summary>
        string FileType { get; }
    }
}
