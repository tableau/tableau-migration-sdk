namespace Tableau.Migration.Engine.Hooks.Transformers
{
    /// <summary>
    /// Interface for an object that can modify or transform content of a specific content type during a migration.
    /// </summary>
    /// <typeparam name="TPublish">The publishable content type.</typeparam>
    public interface IContentTransformer<TPublish> : IMigrationHook<TPublish>
    { }
}
