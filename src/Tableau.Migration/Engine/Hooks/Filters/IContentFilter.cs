using System.Collections.Generic;

namespace Tableau.Migration.Engine.Hooks.Filters
{
    /// <summary>
    /// Interface for an object that can filter content of a specific content type, to determine which content to migrate.
    /// </summary>
    /// <typeparam name="TContent">Type of entity to be filtered</typeparam>
    public interface IContentFilter<TContent>
        : IMigrationHook<IEnumerable<ContentMigrationItem<TContent>>>
        where TContent : IContentReference
    { }
}
