using System;

namespace Tableau.Migration.Content
{
    /// <summary>
    /// Interface for a site.
    /// </summary>
    public interface ISite //We don't implement IContentReference because sites aren't a true 'content type.'
    {
        /// <summary>
        /// Gets the unique identifier of the site, 
        /// corresponding to the LUID in the Tableau REST API.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets the site's friendly name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets the site's "content URL".
        /// </summary>
        string ContentUrl { get; }
    }
}