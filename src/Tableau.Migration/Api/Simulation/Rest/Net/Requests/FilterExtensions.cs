using System.Collections.Generic;
using System.Linq;
using Tableau.Migration.Net.Rest.Filtering;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Requests
{
    internal static class FilterExtensions
    {
        public static Filter? GetFilter(this IEnumerable<Filter> filters, string field, string? @operator = null)
        {
            return filters
                .LastOrDefault(f =>
                    f.Field == field &&
                    (@operator is null || f.Operator == @operator));
        }

        public static string? GetFilterValue(this IEnumerable<Filter> filters, string field, string? @operator = null)
            => filters.GetFilter(field, @operator)?.Value.ToString();
    }
}
