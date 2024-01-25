using Tableau.Migration.Engine.Options;

namespace Tableau.Migration.Engine.Hooks.Filters.Default
{
    /// <summary>
    /// Migration filter that skips previously migrated content.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    public class PreviouslyMigratedFilter<TContent> 
        : ContentFilterBase<TContent> where TContent : IContentReference
    {
        /// <summary>
        /// Creates a new <see cref="PreviouslyMigratedFilter{TContent}"/> object.
        /// </summary>
        /// <param name="input">The migration input.</param>
        /// <param name="optionsProvider">The options provider.</param>
        public PreviouslyMigratedFilter(IMigrationInput input, IMigrationPlanOptionsProvider<PreviouslyMigratedFilterOptions> optionsProvider)
        {
            Disabled = input.PreviousManifest is null || optionsProvider.Get().Disabled;
        }

        /// <inheritdoc />
        public override bool ShouldMigrate(ContentMigrationItem<TContent> item)
        {
            //The item's manifest entry will be set to migrated if a previous manifest
            //was used and the item successfully migrated.
            return !(item.ManifestEntry.HasMigrated);
        }
    }
}
