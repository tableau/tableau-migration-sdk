using Tableau.Migration.Engine.Hooks;

namespace Tableau.Migration.Engine.Migrators
{
    /// <summary>
    /// Interface representing a hook called for a single content item.
    /// </summary>
    public interface IContentMigrationHook<TContent> : IMigrationHook<IContentItemMigrationResult<TContent>>
    { }
}
