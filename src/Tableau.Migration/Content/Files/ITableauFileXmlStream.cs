using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Tableau.Migration.Content.Files
{
    /// <summary>
    /// Interface that represents a read/write stream to a Tableau XML file.
    /// </summary>
    public interface ITableauFileXmlStream : IAsyncDisposable
    {
        /// <summary>
        /// Gets a read/write stream to the XML content.
        /// </summary>
        Stream XmlContent { get; }

        /// <summary>
        /// Gets the currently loaded XML of the file,
        /// parsing the file if necessary.
        /// Changes to the XML will be automatically saved before publishing.
        /// </summary>
        /// <param name="cancel">A cancellation token to obey.</param>
        /// <returns>The XML document.</returns>
        Task<XDocument> GetXmlAsync(CancellationToken cancel);
    }
}
