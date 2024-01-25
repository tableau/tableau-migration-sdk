using System.Collections.Generic;

namespace Tableau.Migration.Net.Rest.Filtering
{
    /// <summary>
    /// <para>
    /// Interface for a class that can build REST API filter query strings.
    /// </para>
    /// <para>
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_concepts_filtering_and_sorting.htm#filtering">Tableau API Reference</see> for more details.
    /// </para>
    /// </summary>
    public interface IFilterBuilder
    {
        /// <summary>
        /// Gets whether the builder contains any filters.
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Adds a filter to the builder.
        /// </summary>
        /// <param name="filter">The filter to add.</param>
        /// <returns>The current <see cref="IFilterBuilder"/> instance.</returns>
        IFilterBuilder AddFilter(Filter filter);

        /// <summary>
        /// Adds filters to the builder.
        /// </summary>
        /// <param name="filters">The filters to add.</param>
        /// <returns>The current <see cref="IFilterBuilder"/> instance.</returns>
        IFilterBuilder AddFilters(params Filter[] filters);

        /// <summary>
        /// Adds filters to the builder.
        /// </summary>
        /// <param name="filters">The filters to add.</param>
        /// <returns>The current <see cref="IFilterBuilder"/> instance.</returns>
        IFilterBuilder AddFilters(IEnumerable<Filter> filters);

        /// <summary>
        /// Builds the string value for the filters for use in query strings.
        /// </summary>
        /// <returns>The formatted string representation of the filters.</returns>
        string Build();

        /// <summary>
        /// Appends the added filters to the specified <see cref="IQueryStringBuilder"/> instance.
        /// </summary>
        /// <param name="builder">The <see cref="IQueryStringBuilder"/> to append to.</param>
        void AppendQueryString(IQueryStringBuilder builder);
    }
}