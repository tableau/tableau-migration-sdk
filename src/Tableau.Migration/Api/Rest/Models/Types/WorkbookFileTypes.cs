namespace Tableau.Migration.Api.Rest.Models.Types
{
    /// <summary>
    /// <para>
    /// Class containing workbook file type constants.
    /// </para>
    /// <para>
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_publishing.htm#publish_workbook">Tableau API Reference</see> for documentation.
    /// </para>
    /// </summary>
    public class WorkbookFileTypes : StringEnum<WorkbookFileTypes>
    {
        /// <summary>
        /// Gets the name of the twb workbook file type.
        /// </summary>
        public const string Twb = "twb";

        /// <summary>
        /// Gets the name of the twbx workbook file type.
        /// </summary>
        public const string Twbx = "twbx";
    }
}

