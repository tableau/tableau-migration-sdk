using System.Collections.Immutable;
using System.Linq;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Options;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Hooks.Filters.Default
{
    /// <summary>
    /// <see cref="IContentFilter{IGroup}"/> implementation used to filter out the default "All Users" group. 
    /// </summary>
    public sealed class GroupAllUsersFilter : ContentFilterBase<IGroup>
    {
        private readonly IImmutableList<string> _allUsersTranslations;

        /// <summary>
        /// Creates a new <see cref="GroupAllUsersFilter"/> instance.
        /// </summary>
        /// <param name="optionsProvider">The filter options provider.</param>
        public GroupAllUsersFilter(IMigrationPlanOptionsProvider<GroupAllUsersFilterOptions> optionsProvider)
        {
            var options = optionsProvider.Get();

            _allUsersTranslations = AllUsersTranslations.GetAll(options.AllUsersGroupNames);
        }

        /// <inheritdoc/>
        public override bool ShouldMigrate(ContentMigrationItem<IGroup> item)
        {
            // Special-casing for English since it'll be the most common.
            if (item.SourceItem.Name is AllUsersTranslations.English ||
                _allUsersTranslations.Contains(item.SourceItem.Name))
            {
                return false;
            }

            return true;
        }
    }
}
