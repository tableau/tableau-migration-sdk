//
//  Copyright (c) 2025, Salesforce, Inc.
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
    /// Class representing a flow update response.
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_flow.htm#update_flow">Tableau API Reference</see> for documentation.
    /// </summary>
    [XmlType(XmlTypeName)]
    public class UpdateFlowResponse : TableauServerResponse<UpdateFlowResponse.FlowType>
    {
        /// <summary>
        /// Gets or sets the flow object.
        /// </summary>
        [XmlElement("flow")]
        public override FlowType? Item { get; set; }

        /// <summary>
        /// Type for the flow object.
        /// </summary>
        public class FlowType : IRestIdentifiable
        {
            /// <summary>
            /// Gets or sets the id for the response.
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
            /// Gets or sets the webpage URL for the response.
            /// </summary>
            [XmlAttribute("webpageUrl")]
            public string? WebpageUrl { get; set; }

            /// <summary>
            /// Gets or sets the file type for the response.
            /// </summary>
            [XmlAttribute("fileType")]
            public string? FileType { get; set; }

            /// <summary>
            /// Gets or sets the creation date/time for the response.
            /// </summary>
            [XmlAttribute("createdAt")]
            public string? CreatedAt { get; set; }

            /// <summary>
            /// Gets or sets the update date/time for the response.
            /// </summary>
            [XmlAttribute("updatedAt")]
            public string? UpdatedAt { get; set; }

            /// <summary>
            /// Gets or sets the project for the response.
            /// </summary>
            [XmlElement("project")]
            public ProjectType? Project { get; set; }

            /// <summary>
            /// Gets or sets the owner for the response.
            /// </summary>
            [XmlElement("owner")]
            public OwnerType? Owner { get; set; }

            /// <summary>
            /// Class representing a REST API project response.
            /// </summary>
            public class ProjectType
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
            }

            /// <summary>
            /// Class representing a REST API owner response.
            /// </summary>
            public class OwnerType
            {
                /// <summary>
                /// Gets or sets the ID for the response.
                /// </summary>
                [XmlAttribute("id")]
                public Guid Id { get; set; }
            }
        }
    }
}

