namespace Tableau.Migration.Net.Rest.Fields
{
    /// <summary>
    /// <para>
    /// Interface for a class that can build REST API field query strings.
    /// </para>
    /// <para>
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_concepts_fields.htm">Tableau API Reference</see> for more details.
    /// </para>
    /// </summary>
    public interface IFieldBuilder
    {
        /// <summary>
        /// Gets whether the builder contains any fields.
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Adds a field to the builder.
        /// </summary>
        /// <param name="field">The field to add.</param>
        /// <returns>The current <see cref="IFieldBuilder"/> instance.</returns>
        IFieldBuilder AddField(Field field);

        /// <summary>
        /// Adds fields to the builder.
        /// </summary>
        /// <param name="fields">The fields to add.</param>
        /// <returns>The current <see cref="IFieldBuilder"/> instance.</returns>
        IFieldBuilder AddFields(params Field[] fields);

        /// <summary>
        /// Builds the string value for the fields for use in query strings.
        /// </summary>
        /// <returns>The formatted string representation of the fields.</returns>
        string Build();

        /// <summary>
        /// Appends the added fields to the specified <see cref="IQueryStringBuilder"/> instance.
        /// </summary>
        /// <param name="builder">The <see cref="IQueryStringBuilder"/> to append to.</param>
        void AppendQueryString(IQueryStringBuilder builder);
    }
}