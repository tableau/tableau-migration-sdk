using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Engine.Hooks.Filters
{
    /// <summary>
    /// Base implementation for an object that can filter content of a specific content type, to determine which content to migrate.
    /// </summary>
    /// <typeparam name="TContent"><inheritdoc/></typeparam>
    public abstract class ContentFilterBase<TContent> : IContentFilter<TContent>
        where TContent : IContentReference
    {
        /// <inheritdoc />
        public Task<IEnumerable<ContentMigrationItem<TContent>>?> ExecuteAsync(IEnumerable<ContentMigrationItem<TContent>> unfilteredItems,
                                                                                     CancellationToken cancel)
        {
            var result = unfilteredItems;

            //Avoid re-allocation on a no-op/disabled filter.
            if (!Disabled)
            {
                result = unfilteredItems.Where(ShouldMigrate);
            }

            return Task.FromResult((IEnumerable<ContentMigrationItem<TContent>>?)result);
        }

        /// <summary>
        /// Gets or sets whether the filter is disabled.
        /// </summary>
        protected virtual bool Disabled { get; set; }

        /// <summary>
        /// Checks if the item should be migrated.
        /// </summary>
        /// <param name="item">The item to evaluate.</param>
        /// <returns>True if the item should be migrated.</returns>
        public abstract bool ShouldMigrate(ContentMigrationItem<TContent> item);
    }
}

