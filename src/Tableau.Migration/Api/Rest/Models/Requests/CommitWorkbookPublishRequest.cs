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
using System.Linq;
using System.Xml.Serialization;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Content;

namespace Tableau.Migration.Api.Rest.Models.Requests
{
    /// <summary>
    /// <para>
    /// Class representing an commit workbook request.
    /// </para>
    /// <para>
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_publishing.htm#publish_workbook">Tableau API Reference</see> for documentation.
    /// </para>
    /// </summary>
    [XmlType(XmlTypeName)]
    public class CommitWorkbookPublishRequest : TableauServerRequest
    {
        /// <summary>
        /// The default parameterless constructor.
        /// </summary>
        public CommitWorkbookPublishRequest()
        { }

        /// <summary>
        /// Creates a new <see cref="CommitWorkbookPublishRequest"/> object.
        /// </summary>
        /// <param name="options">The publish options.</param>
        public CommitWorkbookPublishRequest(IPublishWorkbookOptions options)
        {
            Workbook = new WorkbookType
            {
                Name = options.Name,
                Description = options.Description,
                ShowTabs = options.ShowTabs,
                ThumbnailsUserId = options.ThumbnailsUserId ?? Guid.Empty,
                EncryptExtracts = options.EncryptExtracts,
                Project = new WorkbookType.ProjectType
                {
                    Id = options.ProjectId
                },
                // We're only setting the hidden view names here because any others will be not hidden by default.
                Views = options.HiddenViewNames.Distinct(View.NameComparer).Select(v => new WorkbookType.ViewType
                {
                    Name = v,
                    Hidden = true
                })
                .ToArray()
            };
        }

        /// <summary>
        /// Gets or sets the workbook for the request.
        /// </summary>
        [XmlElement("workbook")]
        public WorkbookType? Workbook { get; set; }

        /// <summary>
        /// The workbook type in the API request body.
        /// </summary>
        public class WorkbookType
        {
            /// <summary>
            /// Gets or sets the name for the workbook.
            /// </summary>
            [XmlAttribute("name")]
            public string? Name { get; set; }
            
            /// <summary>
            /// Gets or sets the description for the workbook.
            /// </summary>
            [XmlAttribute("description")]
            public string? Description { get; set; }

            /// <summary>
            /// Gets or sets whether to show tabs for the workbook.
            /// </summary>
            [XmlAttribute("showTabs")]
            public bool ShowTabs { get; set; }

            /// <summary>
            /// Gets or sets whether to encrypt extracts for the workbook.
            /// </summary>
            [XmlAttribute("encryptExtracts")]
            public bool EncryptExtracts { get; set; }

            /// <summary>
            /// Gets or sets the ID of the user to generate thumbnails as.
            /// </summary>
            [XmlAttribute("thumbnailsUserId")]
            public Guid ThumbnailsUserId { get; set; }

            /// <summary>
            /// Gets or sets the views to hide or show in the request
            /// </summary>
            [XmlArray("connections")]
            [XmlArrayItem("connections")]
            public ConnectionType[] Connections { get; set; } = Array.Empty<ConnectionType>();

            /// <summary>
            /// Gets or sets the workbook's project for the request.
            /// </summary>
            [XmlElement("project")]
            public ProjectType? Project { get; set; }

            /// <summary>
            /// Gets or sets the views to hide or show in the request
            /// </summary>
            [XmlArray("views")]
            [XmlArrayItem("view")]
            public ViewType[] Views { get; set; } = Array.Empty<ViewType>();

            #region - Object Specific Types -

            /// <summary>
            /// The connections type in the API request body.
            /// </summary>
            public class ConnectionType
            {
                /// <summary>
                /// Default parameterless constructor
                /// </summary>
                public ConnectionType()
                { }

                /// <summary>
                /// Gets or sets the server address for the request's project.
                /// </summary>
                [XmlAttribute("serverAddress")]
                public string? ServerAddress { get; set; }

                /// <summary>
                /// Gets or sets the server address for the request's project.
                /// </summary>
                [XmlAttribute("serverPort")]
                public string? ServerPort { get; set; }

                /// <summary>
                /// Gets or sets the server address for the request's project.
                /// </summary>
                [XmlElement("connectionCredentials")]
                public ConnectionCredentialsType? Credentials { get; set; }


                #region - ConnectionType Specific Types -

                /// <summary>
                /// The connection credentials type in the API request body.
                /// </summary>
                public class ConnectionCredentialsType
                {
                    /// <summary>
                    /// Default parameterless constructor
                    /// </summary>
                    public ConnectionCredentialsType()
                    { }

                    /// <summary>
                    /// Gets or sets the connection credentials name for the request's project.
                    /// </summary>
                    [XmlAttribute("name")]
                    public string? Name { get; set; }

                    /// <summary>
                    /// Gets or sets the connection credentials password for the request's project.
                    /// </summary>
                    [XmlAttribute("password")]
                    public string? Password { get; set; }

                    /// <summary>
                    /// Gets or sets the connection credentials embed flag for the request's project.
                    /// </summary>
                    [XmlAttribute("embed")]
                    public string? Embed { get; set; }

                    /// <summary>
                    /// Gets or sets the connection credentials embed flag for the request's project.
                    /// </summary>
                    [XmlAttribute("oAuth")]
                    public string? OAuth { get; set; }
                }


                #endregion
            }

            /// <summary>
            /// The project type in the API request body.
            /// </summary>
            public class ProjectType
            {
                /// <summary>
                /// Gets or sets the ID for the request's project.
                /// </summary>
                [XmlAttribute("id")]
                public Guid Id { get; set; }
            }

            /// <summary>
            /// The views hidden status in the API request body
            /// </summary>
            public class ViewType
            {
                /// <summary>
                /// Default parameterless constructor
                /// </summary>
                public ViewType()
                { }

                /// <summary>
                /// The name of the view to hide or show
                /// </summary>
                [XmlAttribute("name")]
                public string? Name { get; set; }

                /// <summary>
                /// Visibility status of the view
                /// </summary>
                [XmlAttribute("hidden")]
                public bool Hidden { get; set; }
            }

            #endregion
        }
    }
}
