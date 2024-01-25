using System.Collections.Generic;

namespace Tableau.Migration.Net
{
    /// <summary>
    /// Class that can build URL query strings.
    /// </summary>
    public interface IQueryStringBuilder
    {
        /// <summary>
        /// Gets whether any filters have been added to this instance.
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Adds the items from the specified <see cref="IDictionary{String, String}"/> whose keys have not been previously added.
        /// </summary>
        /// <param name="items">The items to add to the query string.</param>
        void AddOrUpdate(IDictionary<string, string> items);

        /// <summary>
        /// Adds the specified key and value if the key has not been previously added.
        /// </summary>
        /// <param name="key">The key to add to the query string.</param>
        /// <param name="value">The value to add to the query string.</param>
        void AddOrUpdate(string key, string value);

        /// <summary>
        /// Builds the resulting query string for the key/value pairs that have been added.
        /// </summary>
        string Build();
    }
}