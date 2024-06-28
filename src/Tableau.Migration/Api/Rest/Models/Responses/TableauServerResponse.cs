//
//  Copyright (c) 2024, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the "License") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using System.Xml.Serialization;
using Tableau.Migration.Api.Rest.Models.Types;

namespace Tableau.Migration.Api.Rest.Models.Responses
{
    /// <summary>
    /// Base class representing REST API responses.
    /// </summary>
    public abstract class TableauServerResponse : ITableauServerResponse
    {
        /// <summary>
        /// Gets the XML type name for Tableau Server REST API responses, i.e. &lt;tsResponse&gt;
        /// </summary>
        internal const string XmlTypeName = "tsResponse";

        /// <inheritdoc />
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
