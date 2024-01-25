namespace Tableau.Migration.Engine.Hooks.PostPublish
{
    /// <summary>
    /// Interface representing a hook called when a migration attempt for a single content item completes.
    /// </summary>
    /// <typeparam name="TPublish">The publish type.</typeparam>
    /// <typeparam name="TResult">The post-publish result type.</typeparam>
    public interface IContentItemPostPublishHook<TPublish, TResult> :
        IMigrationHook<ContentItemPostPublishContext<TPublish, TResult>>
    { }
}
