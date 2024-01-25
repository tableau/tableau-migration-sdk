namespace Tableau.Migration.Api.Rest.Models.Types
{
    /// <summary>
    /// <para>
    /// Class containing data source file type constants.
    /// </para>
    /// <para>
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_data_sources.htm#publish_data_source">Tableau API Reference</see> for documentation.
    /// </para>
    /// </summary>
    public class DataSourceFileTypes : StringEnum<DataSourceFileTypes>
    {
        /// <summary>
        /// Gets the name of the Hyper data source file type.
        /// </summary>
        public const string Hyper = "hyper";

        /// <summary>
        /// Gets the name of the Tds data source file type.
        /// </summary>
        public const string Tds = "tds";

        /// <summary>
        /// Gets the name of the Tdsx data source file type.
        /// </summary>
        public const string Tdsx = "tdsx";

        /// <summary>
        /// Gets the name of the Tde data source file type.
        /// </summary>
        public const string Tde = "tde";
    }
}

