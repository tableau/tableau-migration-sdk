using System.Collections.Immutable;
using Tableau.Migration.Content.Permissions;

namespace Tableau.Migration.Engine.Hooks.Transformers.Default
{
    /// <summary>
    /// Interface for a permissions transformer object.
    /// </summary>
    public interface IPermissionsTransformer : IContentTransformer<IImmutableList<IGranteeCapability>>
    { }
}