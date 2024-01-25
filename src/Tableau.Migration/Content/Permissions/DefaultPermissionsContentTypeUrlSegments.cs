namespace Tableau.Migration.Content.Permissions
{
    /// <summary>
    /// <para>
    /// Class containing default project permission content type URL segment constants.
    /// </para>
    /// <para>
    /// For example, for the URL "/api/api-version/sites/site-luid/projects/project-luid/default-permissions/workbooks" the URL segment would be "workbooks".
    /// </para>
    /// <para>
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_permissions.htm#query_default_permissions">Tableau API Reference</see> for documentation.
    /// </para>
    /// </summary>
    public class DefaultPermissionsContentTypeUrlSegments : StringEnum<DefaultPermissionsContentTypeUrlSegments>
    {
        /// <summary>
        /// Gets the workbook content type URL path segment.
        /// </summary>
        public const string Workbooks = "workbooks";

        /// <summary>
        /// Gets the data source content type URL path segment.
        /// </summary>
        public const string DataSources = "datasources";

        /// <summary>
        /// Gets the flow content type URL path segment.
        /// </summary>
        public const string Flows = "flows";

        /// <summary>
        /// Gets the metric content type URL path segment.
        /// </summary>
        public const string Metrics = "metrics";

        /// <summary>
        /// Gets the database content type URL path segment.
        /// </summary>
        public const string Databases = "databases";

        /// <summary>
        /// Gets the table content type URL path segment.
        /// </summary>
        public const string Tables = "tables";
    }
}
