using System.Collections.Generic;
using System.Linq;

namespace Tableau.Migration.Net.Rest.Fields
{
    /// <summary>
    /// <para>
    /// Class that can build REST API field query strings.
    /// </para>
    /// <para>
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_concepts_fields.htm">Tableau API Reference</see> for more details.
    /// </para>
    /// </summary>
    internal sealed class FieldBuilder : RestQueryBuilderBase<Field>, IFieldBuilder
    {
        /// <inheritdoc/>
        public IFieldBuilder AddField(Field field)
        {
            _items.Add(field);
            return this;
        }

        /// <inheritdoc/>
        public IFieldBuilder AddFields(params Field[] fields)
        {
            Guard.AgainstNull(fields, nameof(fields));

            foreach (var field in fields)
                AddField(field);

            return this;
        }

        /// <inheritdoc/>
        protected override IDictionary<string, string>? BuildQueryString()
        {
            if (IsEmpty)
                return null;

            return new Dictionary<string, string>
            {
                ["fields"] = string.Join(",", _items.Select(s => s.Expression))
            };
        }
    }
}
