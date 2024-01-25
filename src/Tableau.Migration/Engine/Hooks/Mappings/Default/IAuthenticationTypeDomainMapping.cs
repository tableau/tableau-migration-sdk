using Tableau.Migration.Content;

namespace Tableau.Migration.Engine.Hooks.Mappings.Default
{
    /// <summary>
    /// Interface for a content mapping that maps user and group
    /// domains based on the destination authentication type.
    /// </summary>
    public interface IAuthenticationTypeDomainMapping
        : IContentMapping<IUser>, IContentMapping<IGroup>
    { }
}
