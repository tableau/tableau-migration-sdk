using System.Collections.Generic;
using System.Linq;

namespace Tableau.Migration.Net.Rest.Sorting
{
    /// <summary>
    /// <para>
    /// Class that can build REST API sort query strings.
    /// </para>
    /// <para>
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_concepts_filtering_and_sorting.htm#sorting">Tableau API Reference</see> for more details.
    /// </para>
    /// </summary>
    internal sealed class SortBuilder : RestQueryBuilderBase<Sort>, ISortBuilder
    {
        /// <inheritdoc/>
        public ISortBuilder AddSort(Sort sort)
        {
            _items.Add(sort);
            return this;
        }

        /// <inheritdoc/>
        public ISortBuilder AddSorts(params Sort[] sorts)
        {
            Guard.AgainstNull(sorts, nameof(sorts));

            foreach (var sort in sorts)
                AddSort(sort);

            return this;
        }

        /// <inheritdoc/>
        protected override IDictionary<string, string>? BuildQueryString()
        {
            if (IsEmpty)
                return null;

            return new Dictionary<string, string>
            {
                ["sort"] = string.Join(",", _items.Select(s => s.Expression))
            };
        }
    }
}
