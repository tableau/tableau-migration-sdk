using Tableau.Migration.Content;

namespace Tableau.Migration.Engine.Hooks.Mappings.Default
{
    /// <summary>
    /// Interface for a content mapping that maps users
    /// to supply a Tableau Cloud compatible username.
    /// </summary>
    public interface ITableauCloudUsernameMapping
        : IContentMapping<IUser>
    { }
}
