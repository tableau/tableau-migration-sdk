﻿using System.Xml.Serialization;
using Tableau.Migration.Api.Rest.Models.Types;

namespace Tableau.Migration.Api.Rest.Models.Responses
{
    /// <summary>
    /// Base class representing REST API responses.
    /// </summary>
    public abstract class TableauServerResponse
    {
        /// <summary>
        /// Gets the XML type name for Tableau Server REST API responses, i.e. &lt;tsResponse&gt;
        /// </summary>
        internal const string XmlTypeName = "tsResponse";

        /// <summary>
        /// Gets or sets the error for the response.
        /// </summary>
        [XmlElement("error")]
        public Error? Error { get; set; }

        /// <summary>
        /// Creates a new <see cref="TableauServerResponse"/> instance.
        /// </summary>
        public TableauServerResponse()
        { }

        /// <summary>
        /// Creates a new <see cref="TableauServerResponse"/> instance.
        /// </summary>
        /// <param name="error">The error for the response</param>
        internal TableauServerResponse(Error error)
        {
            Error = error;
        }
    }

    /// <summary>
    /// Base class representing REST API responses.
    /// </summary>
    public abstract class TableauServerResponse<TItem> : TableauServerResponse, ITableauServerResponse<TItem>
    {
        /// <inheritdoc/>
        [XmlIgnore] // Ignored so the derived class can set the XmlElement name.
        public abstract TItem? Item { get; set; }

        /// <summary>
        /// Creates a new <see cref="TableauServerResponse{TItem}"/> instance.
        /// </summary>
        public TableauServerResponse()
            : base()
        { }

        /// <summary>
        /// Creates a new <see cref="TableauServerResponse{TItem}"/> instance.
        /// </summary>
        /// <param name="error">The error for the response</param>
        internal TableauServerResponse(Error error)
            : base(error)
        { }
    }

    /// <summary>
    /// Base class representing REST API responses 
    /// that have a parent content item.
    /// </summary>
    public abstract class TableauServerWithParentResponse<TItem> : TableauServerResponse<TItem>, ITableauServerWithParentResponse<TItem>
    {
        /// <inheritdoc/>
        [XmlElement("parent")]
        public ParentContentType? Parent { get; set; }
    }
}
