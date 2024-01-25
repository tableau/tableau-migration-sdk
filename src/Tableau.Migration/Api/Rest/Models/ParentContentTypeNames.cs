namespace Tableau.Migration.Api.Rest.Models
{
    /// <summary>
    /// Class containing ParentContentTypeName constants for use in permissions.
    /// </summary>
    public class ParentContentTypeNames : StringEnum<ParentContentTypeNames>
    {
        /// <summary>
        /// Gets the name of data source parent content type.
        /// </summary>
        public const string DataSource = "DataSource";

        /// <summary>
        /// Gets the name of flow parent content type.
        /// </summary>
        public const string Flow = "Flow";

        /// <summary>
        /// Gets the name of project parent content type.
        /// </summary>
        public const string Project = "Project";

        /// <summary>
        /// Gets the name of workbook parent content type.
        /// </summary>
        public const string Workbook = "Workbook";
    }
}
