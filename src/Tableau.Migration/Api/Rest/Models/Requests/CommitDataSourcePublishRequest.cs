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
using Tableau.Migration.Api.Models;

namespace Tableau.Migration.Api.Rest.Models.Requests
{
    /// <summary>
    /// <para>
    /// Class representing an commit data source request.
    /// </para>
    /// <para>
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_data_sources.htm#publish_data_source">Tableau API Reference</see> for documentation.
    /// </para>
    /// </summary>
    [XmlType(XmlTypeName)]
    public class CommitDataSourcePublishRequest : TableauServerRequest
    {
        /// <summary>
        /// The default parameterless constructor.
        /// </summary>
        public CommitDataSourcePublishRequest()
        { }

        /// <summary>
        /// Creates a new <see cref="CommitDataSourcePublishRequest"/> object.
        /// </summary>
        /// <param name="options">The publish options.</param>
        public CommitDataSourcePublishRequest(IPublishDataSourceOptions options)
        {
            DataSource = new DataSourceType
            {
                Name = options.Name,
                Description = options.Description,
                UseRemoteQueryAgent = options.UseRemoteQueryAgent,
                EncryptExtracts = options.EncryptExtracts,
                Project = new DataSourceType.ProjectType
                {
                    Id = options.ProjectId
                }
            };
        }

        /// <summary>
        /// Gets or sets the data source for the request.
        /// </summary>
        [XmlElement("datasource")]
        public DataSourceType? DataSource { get; set; }

        /// <summary>
        /// The data source type in the API request body.
        /// </summary>
        public class DataSourceType
        {
            /// <summary>
            /// Gets or sets the name for the data source.
            /// </summary>
            [XmlAttribute("name")]
            public string? Name { get; set; }

            /// <summary>
            /// Gets or sets the description for the data source.
            /// </summary>
            [XmlAttribute("description")]
            public string? Description { get; set; }

            /// <summary>
            /// Gets or sets whether the data source uses Tableau Bridge.
            /// </summary>
            [XmlAttribute("useRemoteQueryAgent")]
            public bool UseRemoteQueryAgent { get; set; }

            /// <summary>
            /// Gets or sets whether to encrypt extracts for the data source.
            /// </summary>
            [XmlAttribute("encryptExtracts")]
            public bool EncryptExtracts { get; set; }

            /// <summary>
            /// Gets or sets the data source's project for the request.
            /// </summary>
            [XmlElement("project")]
            public ProjectType? Project { get; set; }

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
        }
    }
}
