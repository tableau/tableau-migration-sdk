using System.Collections.Generic;
using System.Linq;

namespace Tableau.Migration.Net.Rest.Paging
{
    /// <summary>
    /// <para>
    /// Class that can build REST API field query strings.
    /// </para>
    /// <para>
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_concepts_fields.htm">Tableau API Reference</see> for more details.
    /// </para>
    /// </summary>
    internal sealed class PageBuilder : RestQueryBuilderBase<Page>, IPageBuilder
    {
        internal const string PageSizeKey = "pageSize";
        internal const string PageNumberKey = "pageNumber";

        /// <inheritdoc/>
        public IPageBuilder SetPage(Page page)
        {
            _items.Clear();
            _items.Add(page);
            return this;
        }

        /// <inheritdoc/>
        protected override IDictionary<string, string>? BuildQueryString()
        {
            if (IsEmpty)
                return null;

            var page = _items.First();

            return new Dictionary<string, string>
            {
                [PageSizeKey] = page.PageSize.ToString(),
                [PageNumberKey] = page.PageNumber.ToString()
            };
        }
    }
}
