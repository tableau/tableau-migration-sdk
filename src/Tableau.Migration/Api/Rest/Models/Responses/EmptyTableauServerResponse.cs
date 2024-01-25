using System.Xml.Serialization;

namespace Tableau.Migration.Api.Rest.Models.Responses
{
    /// <summary>
    /// Empty container class for Tableau Server responses
    /// </summary>
    [XmlType(XmlTypeName)]
    public class EmptyTableauServerResponse : TableauServerResponse
    {
        /// <summary>
        /// Creates a new <see cref="EmptyTableauServerResponse"/> instance.
        /// </summary>
        public EmptyTableauServerResponse()
            : base()
        { }

        /// <summary>
        /// Creates a new <see cref="EmptyTableauServerResponse"/> instance.
        /// </summary>
        /// <param name="error">The error for the response</param>
        internal EmptyTableauServerResponse(Error error)
            : base(error)
        { }
    }
}
