using System;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Content;

namespace Tableau.Migration.Engine.Hooks.Transformers.Default
{
    /// <summary>
    /// Transformer that changes the SiteRole of the user. 
    /// <see cref="SiteRoles.ServerAdministrator"/> on Server changes to <see cref="SiteRoles.SiteAdministratorCreator"/> on the cloud.
    /// See <see href="https://help.tableau.com/current/blueprint/en-gb/bp_administrative_roles_responsibilities.htm">Tableau API Reference</see> for details.
    /// </summary>
    public class UserTableauCloudSiteRoleTransformer : ContentTransformerBase<IUser>
    {
        /// <summary>
        /// Creates a new <see cref="UserTableauCloudSiteRoleTransformer"/> object.
        /// </summary>        
        public UserTableauCloudSiteRoleTransformer()
        { }

        /// <inheritdoc />
        public override Task<IUser?> ExecuteAsync(IUser itemToTransform, CancellationToken cancel)
        {
            if (string.Equals(itemToTransform.SiteRole, SiteRoles.ServerAdministrator, StringComparison.OrdinalIgnoreCase))
                itemToTransform.SiteRole = SiteRoles.SiteAdministratorCreator;

            return Task.FromResult<IUser?>(itemToTransform);
        }
    }
}
