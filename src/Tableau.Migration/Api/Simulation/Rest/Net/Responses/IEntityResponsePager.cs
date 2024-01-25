using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net.Http;
using Tableau.Migration.Net.Rest.Paging;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Responses
{
    internal interface IEntityResponsePager<TEntity>
    {
        Page GetPageOptions(HttpRequestMessage request);

        ImmutableArray<TEntity> GetPage(IEnumerable<TEntity> entities, Page pageOptions);
    }
}
