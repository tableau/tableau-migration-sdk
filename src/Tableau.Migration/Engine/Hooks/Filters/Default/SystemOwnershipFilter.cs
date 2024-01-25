using Tableau.Migration.Content;

namespace Tableau.Migration.Engine.Hooks.Filters.Default
{
    /// <summary>
    /// <see cref="IContentFilter{TContent}"/> implementation used to filter out built-in assets
    /// that are under system user ownership, like the Default project.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    public sealed class SystemOwnershipFilter<TContent> : ContentFilterBase<TContent>
        where TContent : IWithOwner
    {
        /// <inheritdoc />
        public override bool ShouldMigrate(ContentMigrationItem<TContent> item)
            => item.SourceItem.Owner.Location != Constants.SystemUserLocation;
    }
}
