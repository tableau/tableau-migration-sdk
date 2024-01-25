namespace Tableau.Migration.Engine.Hooks.Filters
{
    /// <summary>
    /// Base filter for content specific filters. 
    /// Filters based on the <see cref="ContentLocation"/> field.
    /// </summary>
    public class ContentLocationInPathFilter<TContent> : ContentFilterBase<TContent>
        where TContent : IContentReference
    {
        /// <summary>
        /// Path for the filter.
        /// </summary>
        public readonly string Path;

        /// <summary>
        /// Default constructor for <see cref="ContentLocationInPathFilter{TContent}"/>.
        /// </summary>
        /// <param name="path"></param>
        public ContentLocationInPathFilter(string path) => Path = path;

        /// <inheritdoc/>
        public override bool ShouldMigrate(ContentMigrationItem<TContent> item)
            => Path.Contains(item.SourceItem.Location.Path);
    }
}
