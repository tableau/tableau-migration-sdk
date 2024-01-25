namespace Tableau.Migration.Net.Rest.Fields
{
    /// <summary>
    /// <para>
    /// Class representing a REST API field
    /// </para>
    /// <para>
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_concepts_fields.htm">Tableau API Reference</see> for more details.
    /// </para>
    /// </summary>
    public class Field
    {
        /// <summary>
        /// <para>
        /// Gets a <see cref="Field"/> representing "_all_" fields.
        /// </para>
        /// <para>
        /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_concepts_fields.htm#field-expression-syntax">Tableau API Reference</see> for more details.
        /// </para>
        /// </summary>
        public static Field All { get; } = new("_all_");

        /// <summary>
        /// <para>
        /// Gets a <see cref="Field"/> representing "_default_" fields.
        /// </para>
        /// <para>
        /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_concepts_fields.htm#field-expression-syntax">Tableau API Reference</see> for more details.
        /// </para>
        /// </summary>
        public static Field Default { get; } = new("_default_");

        /// <summary>
        /// Gets the field name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the field's expression for use in query strings.
        /// </summary>
        public string Expression { get; }

        /// <summary>
        /// Creates a new <see cref="Field"/> instance.
        /// </summary>
        /// <param name="name">The field name</param>
        public Field(string name)
        {
            Name = name;
            Expression = Name;
        }
    }
}
