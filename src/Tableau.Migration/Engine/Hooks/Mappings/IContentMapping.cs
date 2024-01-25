namespace Tableau.Migration.Engine.Hooks.Mappings
{
    /// <summary>
    /// Interface for an object that can map content of a specific content type.
    /// </summary>
    /// <typeparam name="TContent">Type of entity to be mapped.</typeparam>
    public interface IContentMapping<TContent> : IMigrationHook<ContentMappingContext<TContent>>
        where TContent : IContentReference
    { }
}
