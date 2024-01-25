using System.Collections.Immutable;

namespace Tableau.Migration.Content
{
    /// <summary>
    /// Interface to be inherited by content items with tags.
    /// </summary>
    public interface IWithViews
    {
        /// <summary>
        /// Gets the views for the content item.
        /// </summary>
        IImmutableList<IView> Views { get; }
    }
}