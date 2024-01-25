namespace Tableau.Migration.Content
{
    /// <summary>
    /// Interface for a content item that uses a domain qualified username.
    /// </summary>
    public interface IUsernameContent
        : IContentReference, IWithDomain, IMappableContent
    { }
}
