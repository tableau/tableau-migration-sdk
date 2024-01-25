namespace Tableau.Migration.Net.Rest.Sorting
{
    /// <summary>
    /// <para>
    /// Interface for a class that can build REST API sort query strings.
    /// </para>
    /// <para>
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_concepts_filtering_and_sorting.htm#sorting">Tableau API Reference</see> for more details.
    /// </para>
    /// </summary>
    public interface ISortBuilder
    {
        /// <summary>
        /// Gets whether the builder contains any sorts.
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Adds a sort to the builder.
        /// </summary>
        /// <param name="sort">The sort to add.</param>
        /// <returns>The current <see cref="ISortBuilder"/> instance.</returns>
        ISortBuilder AddSort(Sort sort);

        /// <summary>
        /// Adds sorts to the builder.
        /// </summary>
        /// <param name="sorts">The sorts to add.</param>
        /// <returns>The current <see cref="ISortBuilder"/> instance.</returns>
        ISortBuilder AddSorts(params Sort[] sorts);

        /// <summary>
        /// Builds the string value for the sorts for use in query strings.
        /// </summary>
        /// <returns>The formatted string representation of the sorts.</returns>
        string Build();

        /// <summary>
        /// Appends the added sorts to the specified <see cref="IQueryStringBuilder"/> instance.
        /// </summary>
        /// <param name="builder">The <see cref="IQueryStringBuilder"/> to append to.</param>
        void AppendQueryString(IQueryStringBuilder builder);
    }
}