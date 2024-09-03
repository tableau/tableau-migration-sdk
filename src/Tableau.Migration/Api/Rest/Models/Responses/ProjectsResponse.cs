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
    /// Class representing a projects response.
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_projects.htm#query_projects">Tableau API Reference</see> for documentation.
    /// </summary>
    [XmlType(XmlTypeName)]
    public class ProjectsResponse : PagedTableauServerResponse<ProjectsResponse.ProjectType>
    {
        /// <summary>
        /// Gets or sets the groups for the response.
        /// </summary>
        [XmlArray("projects")]
        [XmlArrayItem("project")]
        public override ProjectType[] Items { get; set; } = Array.Empty<ProjectType>();

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
            /// Gets or sets the topLevelProject flag for the response.
            /// </summary>
            [XmlAttribute("topLevelProject")]
            public bool TopLevelProject { get; set; }

            /// <summary>
            /// Gets or sets the writeable flag for the response.
            /// </summary>
            [XmlAttribute("writeable")]
            public bool Writeable { get; set; }

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
            /// Gets or sets the created timestamp for the response.
            /// </summary>
            [XmlAttribute("createdAt")]
            public string? CreatedAt { get; set; }

            /// <summary>
            /// Gets or sets the updated timestamp for the response.
            /// </summary>
            [XmlAttribute("updatedAt")]
            public string? UpdatedAt { get; set; }

            /// <summary>
            /// Gets or sets the contentPermissions for the response.
            /// </summary>
            [XmlAttribute("contentPermissions")]
            public string? ContentPermissions { get; set; }

            /// <summary>
            /// Gets or sets the contentPermissions for the response.
            /// Gets or sets the parentProjectId for the response.
            /// </summary>
            /// <remarks>
            /// Does not parse due to .NET limitations with nullable XML deserialization.
            /// Use <see cref="IProjectTypeExtensions.GetParentProjectId"/> to get a parsed value.
            /// </remarks>
            [XmlAttribute("parentProjectId")]
            public string? ParentProjectId { get; set; }

            /// <summary>
            /// Gets or sets the owner for the response.
            /// </summary>
            [XmlElement("owner")]
            public UserType? Owner { get; set; }

            ///<inheritdoc/>
            IRestIdentifiable? IWithOwnerType.Owner => Owner;

            /// <summary>
            /// Gets or sets the contentCounts for the response.
            /// </summary>
            [XmlElement("contentCounts")]
            public ContentCountsType? ContentCounts { get; set; }

            #region - Object specific types -

            /// <summary>
            /// Class representing a REST API user response.
            /// </summary>
            public class UserType : IRestIdentifiable
            {
                /// <summary>
                /// Gets or sets the ID for the response.
                /// </summary>
                [XmlAttribute("id")]
                public Guid Id { get; set; }

                /// <summary>
                /// Gets or sets the name for the response.
                /// </summary>
                [XmlAttribute("name")]
                public string? Name { get; set; }

                /// <summary>
                /// Gets or sets the name for the response.
                /// </summary>
                [XmlAttribute("fullName")]
                public string? FullName { get; set; }

                /// <summary>
                /// Gets or sets the email for the response.
                /// </summary>
                [XmlAttribute("email")]
                public string? Email { get; set; }

                /// <summary>
                /// Gets or sets the email for the response.
                /// </summary>
                [XmlAttribute("lastLogin")]
                public string? LastLogin { get; set; }

                /// <summary>
                /// Gets or sets the siteRole for the response.
                /// </summary>
                [XmlAttribute("siteRole")]
                public string? SiteRole { get; set; }
            }

            /// <summary>
            /// Class representing a REST API user response.
            /// </summary>
            public class ContentCountsType
            {
                /// <summary>
                /// Gets or sets the projectCount for the response.
                /// </summary>
                [XmlAttribute("projectCount")]
                public string? ProjectCount { get; set; }

                /// <summary>
                /// Gets or sets the workbookCount for the response.
                /// </summary>
                [XmlAttribute("workbookCount")]
                public string? WorkbookCount { get; set; }

                /// <summary>
                /// Gets or sets the viewCount for the response.
                /// </summary>
                [XmlAttribute("viewCount")]
                public string? ViewCount { get; set; }

                /// <summary>
                /// Gets or sets the datasourceCount for the response.
                /// </summary>
                [XmlAttribute("datasourceCount")]
                public string? DataSourceCount { get; set; }
            }

            #endregion
        }
    }
}
