using System.Collections.Generic;
using System.Linq;

namespace Tableau.Migration.Net.Rest.Filtering
{
    /// <summary>
    /// <para>
    /// Class that can build REST API filter query strings.
    /// </para>
    /// <para>
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_concepts_filtering_and_sorting.htm#filtering">Tableau API Reference</see> for more details.
    /// </para>
    /// </summary>
    internal sealed class FilterBuilder : RestQueryBuilderBase<Filter>, IFilterBuilder
    {
        /// <inheritdoc/>
        public IFilterBuilder AddFilter(Filter filter)
        {
            _items.Add(filter);
            return this;
        }

        /// <inheritdoc/>
        public IFilterBuilder AddFilters(params Filter[] filters)
            => AddFilters((IEnumerable<Filter>)filters);

        /// <inheritdoc/>
        public IFilterBuilder AddFilters(IEnumerable<Filter> filters)
        {
            foreach (var filter in filters)
                AddFilter(filter);

            return this;
        }

        /// <inheritdoc/>
        protected override IDictionary<string, string>? BuildQueryString()
        {
            if (IsEmpty)
                return null;

            return new Dictionary<string, string>
            {
                ["filter"] = string.Join(",", _items.Select(s => s.Expression))
            };
        }
    }
}
