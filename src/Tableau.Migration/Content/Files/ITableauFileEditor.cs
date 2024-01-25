using System;
using System.IO;
using System.IO.Compression;

namespace Tableau.Migration.Content.Files
{
    /// <summary>
    /// Interface for an object that can edit content in Tableau file formats 
    /// including TDS, TDSX, TWB, and TWBX.
    /// All changes made to the content is persisted upon disposal
    /// </summary>
    public interface ITableauFileEditor : IAsyncDisposable
    {
        /// <summary>
        /// Gets the memory backed stream
        /// with unencrypted tableau file data 
        /// to write back to the file store upon disposal.
        /// </summary>
        MemoryStream Content { get; }

        /// <summary>
        /// Gets the zip archive for the file,
        /// or null if the file is an unzipped XML file (TDS or TWB).
        /// The zip archive is backed by the <see cref="Content"/> stream.
        /// </summary>
        ZipArchive? Archive { get; }

        /// <summary>
        /// Gets the current read/write stream to the XML content of the Tableau file,
        /// opening a new stream if necessary.
        /// Changes to the stream will be automatically saved before publishing.
        /// </summary>
        /// <returns>The XML stream to edit.</returns>
        ITableauFileXmlStream GetXmlStream();
    }
}
