namespace Tableau.Migration.Engine.Hooks.PostPublish
{
    /// <summary>
    /// Interface representing a hook called when a migration attempt for bulk content items completes.
    /// </summary>
    public interface IBulkPostPublishHook<TSource> :
        IMigrationHook<BulkPostPublishContext<TSource>>
    { }
}
