namespace Tableau.Migration.Engine.Hooks.Transformers.Default
{
    /// <summary>
    /// Interface for an object that can transform a user reference, for example to maintain owner references.
    /// </summary>
    public interface IUserReferenceTransformer : IContentTransformer<IContentReference>
    { }
}
