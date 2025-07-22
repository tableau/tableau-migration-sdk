using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Content.Permissions;
using Tableau.Migration.Engine.Hooks.Transformers;
using Tableau.Migration.Resources;

namespace Csharp.ExampleApplication.Hooks.Transformers
{
    #region class
    public class ModifyPermissionsTransformer
        : ContentTransformerBase<IPermissionSet>
    {
        public ModifyPermissionsTransformer(ISharedResourcesLocalizer localizer, ILogger<IContentTransformer<IPermissionSet>> logger) 
            : base(localizer, logger)
        { }

        public override Task<IPermissionSet?> TransformAsync(IPermissionSet itemToTransform, CancellationToken cancel)
        {
            itemToTransform.GranteeCapabilities = itemToTransform.GranteeCapabilities
                .Where(g => g.GranteeType is GranteeType.Group)
                .ToList();

            return Task.FromResult<IPermissionSet?>(itemToTransform);
        }
    }
    #endregion
}
