namespace Tableau.Migration.Api.Rest.Models.Requests
{
    /// <summary>
    /// Base class representing REST API requests.
    /// </summary>
    public abstract class TableauServerRequest
    {
        /// <summary>
        /// Gets the XML type name for Tableau Server REST API requests, i.e. &lt;tsRequest&gt;
        /// </summary>
        internal const string XmlTypeName = "tsRequest";
    }
}
