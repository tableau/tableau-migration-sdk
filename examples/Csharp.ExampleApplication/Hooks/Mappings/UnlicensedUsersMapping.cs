using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Hooks.Mappings;
using Tableau.Migration.Engine.Options;
using Tableau.Migration.Resources;

namespace Csharp.ExampleApplication.Hooks.Mappings
{
    #region class
    public class UnlicensedUsersMapping : ContentMappingBase<IUser>
    {
        private readonly ContentLocation _targetUserLocation; // The target user's location.

        public UnlicensedUsersMapping(
            IMigrationPlanOptionsProvider<UnlicensedUsersMappingOptions> optionsProvider,
            ISharedResourcesLocalizer localizer,
            ILogger<IContentMapping<IUser>> logger) : base(localizer, logger)
        {
            var options = optionsProvider.Get();

            // Build the mapping destination location.
            _targetUserLocation = ContentLocation.ForUsername(options.TargetUserDomain, options.TargetUsername);
        }

        public override async Task<ContentMappingContext<IUser>?> MapAsync(ContentMappingContext<IUser> ctx, CancellationToken cancel)
        {
            // Only map unlicensed users.
            if (!String.Equals(ctx.ContentItem.SiteRole, SiteRoles.Unlicensed, StringComparison.OrdinalIgnoreCase))
            {
                return ctx;
            }

            ctx = ctx.MapTo(_targetUserLocation);

            // Map the source user to the target one.
            return await ctx.ToTask();
        }
    }
    #endregion
}
