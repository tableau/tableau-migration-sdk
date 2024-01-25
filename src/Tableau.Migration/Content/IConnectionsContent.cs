using System.Collections.Immutable;

namespace Tableau.Migration.Content
{
    /// <summary>
    /// Interface for content that has connection metadata.
    /// </summary>
    public interface IConnectionsContent
    {
        /// <summary>
        /// Gets the connection metadata.
        /// Connection metadata is read only because connection metadata should
        /// not be transformed directly. Instead, connections should be modified by either: 
        /// 1) manipulating XML before publishing, or 
        /// 2) updating connection metadata in a post-publish hook.
        /// </summary>
        IImmutableList<IConnection> Connections { get; }
    }
}
