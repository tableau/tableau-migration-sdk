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
    /// Class representing an update project request.
    /// </para>
    /// <para>
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_projects.htm#update_project">Tableau API Reference</see> for documentation.
    /// </para>
    /// </summary>
    [XmlType(XmlTypeName)]
    public class UpdateProjectRequest : TableauServerRequest
    {
        /// <summary>
        /// Creates a new <see cref="UpdateProjectRequest"/> object.
        /// </summary>
        public UpdateProjectRequest() { }

        /// <summary>
        /// Creates a new <see cref="UpdateProjectRequest"/> object.
        /// </summary>
        /// <param name="newName">The new name of the project, or null to not update the name.</param>
        /// <param name="newDescription">The new description of the project, or null to not update the description.</param>
        /// <param name="newParentProjectId">The LUID of a parent project to move the project to, or null to not update the parent project.</param>
        /// <param name="newContentPermissions">The new content permission mode of the project, or null to not update the option.</param>
        /// <param name="newControllingPermissionsProjectId">The LUID of a project to assign to control project permissions, or null to not update the option.</param>
        /// <param name="newOwnerId">The LUID of a user to assign the project to as owner, or null to not update the owner.</param>
        public UpdateProjectRequest(
            string? newName = null,
            string? newDescription = null,
            Guid? newParentProjectId = null,
            string? newContentPermissions = null,
            Guid? newControllingPermissionsProjectId = null,
            Guid? newOwnerId = null)
        {
            Project = new();

            if (newName is not null)
                Project.Name = newName;

            if (newDescription is not null)
                Project.Description = newDescription;

            if (newParentProjectId is not null)
            {
                if (newParentProjectId == Guid.Empty)
                {
                    Project.ParentProjectId = string.Empty;
                }
                else
                {
                    Project.ParentProjectId = newParentProjectId.ToString();
                }
            }

            if (newContentPermissions is not null)
                Project.ContentPermissions = newContentPermissions;

            if (newControllingPermissionsProjectId is not null)
                Project.ControllingPermissionsProjectId = newControllingPermissionsProjectId.ToString();

            if (newOwnerId is not null)
                Project.Owner = new() { Id = newOwnerId.Value };
        }

        /// <summary>
        /// Gets or sets the project for the request.
        /// </summary>
        [XmlElement("project")]
        public ProjectType? Project { get; set; }

        /// <summary>
        /// The project type in the API request body.
        /// </summary>
        public class ProjectType
        {
            /// <summary>
            /// Gets or sets the name for the request.
            /// </summary>
            [XmlAttribute("name")]
            public string? Name { get; set; }

            /// <summary>
            /// Gets or sets the description for the request.
            /// </summary>
            [XmlAttribute("description")]
            public string? Description { get; set; }

            /// <summary>
            /// Gets or sets the parent project ID for the request.
            /// </summary>
            [XmlAttribute("parentProjectId")]
            public string? ParentProjectId { get; set; }

            /// <summary>
            /// Gets or sets the content permission mode for the request.
            /// </summary>
            [XmlAttribute("contentPermissions")]
            public string? ContentPermissions { get; set; }

            /// <summary>
            /// Gets or sets the controlling permissions project ID for the request.
            /// </summary>
            [XmlAttribute("controllingPermissionsProjectId")]
            public string? ControllingPermissionsProjectId { get; set; }

            /// <summary>
            /// Gets or sets the owner for the request.
            /// </summary>
            [XmlElement("owner")]
            public OwnerType? Owner { get; set; }

            /// <summary>
            /// Class representing a owner request.
            /// </summary>
            public class OwnerType
            {
                /// <summary>
                /// Gets or sets the id for the request.
                /// </summary>
                [XmlAttribute("id")]
                public Guid Id { get; set; }
            }
        }
    }
}
