using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Hooks.Mappings;
using Tableau.Migration.Engine.Hooks.Mappings.Default;

namespace Tableau.Migration.Interop.Hooks.Mappings
{
    /// <summary>
    /// 
    /// </summary>
    abstract public class SyncTableauCloudUsernameMapping : ITableauCloudUsernameMapping
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        abstract public ContentMappingContext<IUser>? Execute(ContentMappingContext<IUser> ctx);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="cancel"></param>
        /// <returns></returns>
        public Task<ContentMappingContext<IUser>?> ExecuteAsync(ContentMappingContext<IUser> ctx, CancellationToken cancel) => Task.FromResult(Execute(ctx));
        
    }
}
