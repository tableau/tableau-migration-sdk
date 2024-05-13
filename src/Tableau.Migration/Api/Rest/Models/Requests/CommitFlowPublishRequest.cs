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
    /// Class representing an commit prep flow request.
    /// </para>
    /// <para>
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_flow.htm#publish_flow">Tableau API Reference</see> for documentation.
    /// </para>
    /// </summary>
    [XmlType(XmlTypeName)]
    public class CommitFlowPublishRequest : TableauServerRequest
    {
        /// <summary>
        /// The default parameterless constructor.
        /// </summary>
        public CommitFlowPublishRequest()
        { }

        /// <summary>
        /// Creates a new <see cref="CommitFlowPublishRequest"/> object.
        /// </summary>
        /// <param name="options">The publish options.</param>
        public CommitFlowPublishRequest(IPublishFlowOptions options)
        {
            Flow = new FlowType
            {
                Name = options.Name,
                Description = options.Description,
                Project = new FlowType.ProjectType
                {
                    Id = options.ProjectId
                }
            };
        }

        /// <summary>
        /// Gets or sets the prep flow for the request.
        /// </summary>
        [XmlElement("flow")]
        public FlowType? Flow { get; set; }

        /// <summary>
        /// The prep flow type in the API request body.
        /// </summary>
        public class FlowType
        {
            /// <summary>
            /// Gets or sets the name for the prep flow.
            /// </summary>
            [XmlAttribute("name")]
            public string? Name { get; set; }

            /// <summary>
            /// Gets or sets the description for the prep flow.
            /// </summary>
            [XmlAttribute("description")]
            public string? Description { get; set; }

            /// <summary>
            /// Gets or sets the prep flow's project for the request.
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
