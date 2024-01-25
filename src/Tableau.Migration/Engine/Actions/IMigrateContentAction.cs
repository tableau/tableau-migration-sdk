namespace Tableau.Migration.Engine.Actions
{
    /// <summary>
    /// Interface for a migration action that migrates content for a specific content type.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    public interface IMigrateContentAction<TContent> : IMigrationAction
        where TContent : class, IContentReference
    { }
}
