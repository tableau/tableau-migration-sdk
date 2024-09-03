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

namespace Tableau.Migration.Api.Rest.Models.Responses
{
    /// <summary>
    /// <para>
    /// Class representing a project creation response.
    /// </para>
    /// <para>
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_projects.htm#create_project">Tableau API Reference</see> for documentation
    /// </para>
    /// </summary>
    [XmlType(XmlTypeName)]
    public class CreateProjectResponse : TableauServerResponse<CreateProjectResponse.ProjectType>
    {
        /// <summary>
        /// Gets or sets the project for the response.
        /// </summary>
        [XmlElement("project")]
        public override ProjectType? Item { get; set; }

        /// <summary>
        /// Class representing a project response.
        /// </summary>
        public class ProjectType : IProjectType
        {
            /// <summary>
            /// Gets or sets the ID for the response.
            /// </summary>
            [XmlAttribute("id")]
            public Guid Id { get; set; }

            /// <summary>
            /// Gets or sets the parentProjectId for the response.
            /// </summary>
            /// <remarks>
            /// Does not parse due to .NET limitations with nullable XML deserialization.
            /// Use <see cref="IProjectTypeExtensions.GetParentProjectId"/> to get a parsed value.
            /// </remarks>
            [XmlAttribute("parentProjectId")]
            public string? ParentProjectId { get; set; }

            /// <summary>
            /// Gets or sets the name for the response.
            /// </summary>
            [XmlAttribute("name")]
            public string? Name { get; set; }

            /// <summary>
            /// Gets or sets the description for the response.
            /// </summary>
            [XmlAttribute("description")]
            public string? Description { get; set; }

            /// <summary>
            /// Gets or sets the content permissions for the response.
            /// </summary>
            [XmlAttribute("contentPermissions")]
            public string? ContentPermissions { get; set; }

            /// <summary>
            /// Gets or sets the controllingPermissionsProjectId for the response.
            /// </summary>
            /// <remarks>
            /// Does not parse due to .NET limitations with nullable XML deserialization.
            /// Use <see cref="IProjectTypeExtensions.GetControllingPermissionsProjectId"/> to get a parsed value.
            /// </remarks>
            [XmlAttribute("controllingPermissionsProjectId")]
            public string? ControllingPermissionsProjectId { get; set; }

            /// <summary>
            /// Gets or sets the owner for the response.
            /// </summary>
            [XmlElement("owner")]
            public OwnerType? Owner { get; set; }

            ///<inheritdoc/>
            IRestIdentifiable? IWithOwnerType.Owner => Owner;

            #region - Object Specific Types -

            /// <summary>
            /// Class representing a REST API user response.
            /// </summary>
            public class OwnerType : IRestIdentifiable
            {
                /// <summary>
                /// Gets or sets the ID for the response.
                /// </summary>
                [XmlAttribute("id")]
                public Guid Id { get; set; }
            }

            #endregion
        }
    }
}
