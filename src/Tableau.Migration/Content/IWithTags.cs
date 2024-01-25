using System.Collections.Generic;

namespace Tableau.Migration.Content
{
    /// <summary>
    /// Interface to be inherited by content items with tags.
    /// </summary>
    public interface IWithTags
    {
        /// <summary>
        /// Gets or sets the tags for the content item.
        /// </summary>
        IList<ITag> Tags { get; set; }
    }
}