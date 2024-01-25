using System;
using System.Collections.Generic;
using System.Linq;

namespace Tableau.Migration.Net
{
    internal sealed class QueryStringBuilder : IQueryStringBuilder
    {
        private readonly Dictionary<string, string> _query = new();

        /// <inheritdoc/>
        public bool IsEmpty => _query.Count == 0;

        /// <inheritdoc/>
        public string Build()
        {
            return String.Join("&", _query.Select(p => $"{p.Key.UrlEncode()}={p.Value.UrlEncode()}"));
        }

        /// <inheritdoc/>
        public void AddOrUpdate(string query)
        {
            var parts = query.Split('=', 2);

            AddOrUpdate(parts[0].Trim(), parts[1].Trim());
        }

        /// <inheritdoc/>
        public void AddOrUpdate(IDictionary<string, string> items)
        {
            foreach (var item in items)
                AddOrUpdate(item.Key, item.Value);
        }

        /// <inheritdoc/>
        public void AddOrUpdate(string key, string value)
        {
            _query[key] = value;
        }
    }
}
