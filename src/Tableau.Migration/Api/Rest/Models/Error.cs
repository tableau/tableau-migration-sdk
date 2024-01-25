using System;
using System.Xml.Serialization;

namespace Tableau.Migration.Api.Rest.Models
{
    /// <summary>    
    /// Class representing an error response.
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_concepts_errors.htm">Tableau API Reference</see> 
    /// for more details.
    /// </summary>
    [XmlType]
    public class Error
    {
        /// <summary>
        /// Gets or sets the error code for the response.
        /// </summary>
        [XmlAttribute("code")]
        public string? Code { get; set; }

        /// <summary>
        /// Gets or sets the summary for the response.
        /// </summary>
        [XmlElement("summary")]
        public string? Summary { get; set; }

        /// <summary>
        /// Gets or sets a text description for the response.
        /// </summary>
        [XmlElement("detail")]
        public string? Detail { get; set; }

        /// <summary>
        /// Pretty prints the error with the <see cref="Code"/>, <see cref="Summary"/>, and <see cref="Detail"/>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Code} - {Summary}: {Detail}";
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return obj is Error error &&
                   Code == error.Code &&
                   Summary == error.Summary &&
                   Detail == error.Detail;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(Code, Summary, Detail);
        }
    }
}
