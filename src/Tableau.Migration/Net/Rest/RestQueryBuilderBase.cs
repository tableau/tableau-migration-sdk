using System.Collections.Generic;
using System.Linq;

namespace Tableau.Migration.Net.Rest
{
    /// <summary>
    /// Base class for REST query string builders.
    /// </summary>
    /// <typeparam name="TItem">The type of item to format in the query string.</typeparam>
    internal abstract class RestQueryBuilderBase<TItem>
    {
        protected readonly HashSet<TItem> _items = new();

        /// <summary>
        /// Gets whether the builder contains any items.
        /// </summary>
        public bool IsEmpty => _items.Count == 0;

        /// <summary>
        /// Gets the query string key/value pair for the added items.
        /// </summary>
        /// <returns>A key/value pair for the added items.</returns>
        protected abstract IDictionary<string, string>? BuildQueryString();

        /// <summary>
        /// Builds the string value for the items for use in query strings.
        /// </summary>
        /// <returns>The formatted string representation of the items.</returns>
        public string Build()
        {
            var query = BuildQueryString();

            if (query is null)
                return string.Empty;

            return string.Join("&", query.Select(q => string.Join("=", q.Key, q.Value)));
        }

        /// <summary>
        /// Appends the added items to the specified <see cref="IQueryStringBuilder"/> instance.
        /// </summary>
        /// <param name="builder">The <see cref="IQueryStringBuilder"/> to append to.</param>
        public void AppendQueryString(IQueryStringBuilder builder)
        {
            var query = BuildQueryString();

            if (query is null)
                return;

            foreach (var kvp in query)
                builder.AddOrUpdate(kvp.Key, kvp.Value);
        }
    }
}
