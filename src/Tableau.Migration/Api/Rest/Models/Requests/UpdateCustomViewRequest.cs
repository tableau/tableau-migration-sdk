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

using System;
using System.Xml.Serialization;

namespace Tableau.Migration.Api.Rest.Models.Requests
{
    /// <summary>
    /// <para>
    /// Class representing an update custom view request.
    /// </para>
    /// <para>
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_workbooks_and_views.htm#update_custom_view">Tableau API Reference</see> for documentation.
    /// </para>
    /// </summary>
    [XmlType(XmlTypeName)]
    public class UpdateCustomViewRequest : TableauServerRequest
    {
        /// <summary>
        /// The default parameterless constructor.
        /// </summary>
        public UpdateCustomViewRequest() { }

        /// <summary>
        /// Builds the update request for a custom view.
        /// </summary>
        /// <param name="newOwnerId">(Optional) The new owner ID for the custom view.</param>
        /// <param name="newName">(Optional) The new name for the custom view.</param>
        public UpdateCustomViewRequest(
            Guid? newOwnerId = null,
            string? newName = null)
        {
            if (newOwnerId is not null &&
                newOwnerId != Guid.Empty)
            {
                CustomView.Owner.Id = newOwnerId.Value;
            }   

            if (!string.IsNullOrWhiteSpace(newName))
            {
                CustomView.Name = newName;
            }
        }

        /// <summary>
        /// Gets or sets the custom view for the request.
        /// </summary>
        [XmlElement("customView")]
        public CustomViewType CustomView { get; set; } = new CustomViewType();

        /// <summary>
        /// The custom view type in the API request body.
        /// </summary>
        public class CustomViewType
        {
            /// <summary>
            /// Gets or sets the name for the request.
            /// </summary>
            [XmlAttribute("name")]
            public string? Name { get; set; } = null;

            /// <summary>
            /// Gets or sets the owner for the request.
            /// </summary>
            [XmlElement("owner")]
            public OwnerType Owner { get; set; } = new OwnerType();

            /// <summary>
            /// The custom view type in the API request body.
            /// </summary>
            public class OwnerType
            {
                private Guid? _id;

                /// <summary>
                /// Gets or sets the ID for the request.
                /// </summary>
                [XmlAttribute("id")]
                public Guid Id
                {
                    get => _id!.Value;
                    set => _id = value;
                }

                /// <summary>
                /// Defines the serialization for the property <see cref="Id"/>.
                /// </summary>
                [XmlIgnore]
                public bool IdSpecified => _id.HasValue;
            }
        }
    }
}
