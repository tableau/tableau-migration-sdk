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

namespace Tableau.Migration.Api.Rest.Models.Requests
{
    /// <summary>
    /// Class representing an update flow request.
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_flow.htm#update_flow">Tableau API Reference</see> for documentation.
    /// </summary>
    [XmlType(XmlTypeName)]
    public class UpdateFlowRequest : TableauServerRequest
    {
        /// <summary>
        /// The default parameterless constructor.
        /// </summary>
        public UpdateFlowRequest() { }

        /// <summary>
        /// Builds the Update request for a flow.
        /// </summary>
        /// <param name="newProjectId">The LUID of a project to move the flow to, or null to not update the project.</param>
        /// <param name="newOwnerId">The LUID of a user to assign the flow to as owner, or null to not update the owner.</param>
        public UpdateFlowRequest(
            Guid? newProjectId = null,
            Guid? newOwnerId = null)
        {
            Flow = new();

            if (newProjectId is not null)
                Flow.Project = new() { Id = newProjectId.Value };

            if (newOwnerId is not null)
                Flow.Owner = new() { Id = newOwnerId.Value };
        }

        /// <summary>
        /// Gets or sets the flow for the request.
        /// </summary>
        [XmlElement("flow")]
        public FlowType? Flow { get; set; }

        /// <summary>
        /// The flow type in the API request body.
        /// </summary>
        public class FlowType
        {
            /// <summary>
            /// Default parameterless constructor.
            /// </summary>
            public FlowType() { }

            /// <summary>
            /// Class representing a project request.
            /// </summary>
            public class ProjectType
            {
                /// <summary>
                /// Gets or sets the id for the request.
                /// </summary>
                [XmlAttribute("id")]
                public Guid Id { get; set; }
            }

            /// <summary>
            /// Class representing an owner request.
            /// </summary>
            public class OwnerType
            {
                /// <summary>
                /// Gets or sets the id for the request.
                /// </summary>
                [XmlAttribute("id")]
                public Guid Id { get; set; }
            }

            /// <summary>
            /// Gets or sets the project for the request.
            /// </summary>
            [XmlElement("project")]
            public ProjectType? Project { get; set; }

            /// <summary>
            /// Gets or sets the owner for the request.
            /// </summary>
            [XmlElement("owner")]
            public OwnerType? Owner { get; set; }
        }
    }
}

