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
    /// Class representing an update workbook request.
    /// </para>
    /// <para>
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_workbooks_and_views.htm#update_workbook">Tableau API Reference</see> for documentation.
    /// </para>
    /// </summary>
    [XmlType(XmlTypeName)]
    public class UpdateWorkbookRequest : TableauServerRequest
    {
        /// <summary>
        /// The default parameterless constructor.
        /// </summary>
        public UpdateWorkbookRequest() { }

        /// <summary>
        /// Builds the Update request for a workbook.
        /// </summary>
        /// <param name="newName">The new name of the workbook, or null to not update the name.</param>
        /// /// <param name="newDescription">The new description of the workbook, or null to not update the description.</param>
        /// <param name="newProjectId">The LUID of a project to move the workbook to, or null to not update the project.</param>
        /// <param name="newOwnerId">The LUID of a user to assign the workbook to as owner, or null to not update the owner.</param>
        /// <param name="newShowTabs">Whether or not to show workbook views in tabs, or null to not update the option.</param>
        /// <param name="newRecentlyViewedFlag">Whether or not to show the workbook in the recently viewed list, or null to not update the flag.</param>
        /// <param name="newEncryptExtracts">Whether or not to encrypt extracts, or null to not update the option.</param>
        public UpdateWorkbookRequest(
            string? newName = null,
            string? newDescription = null,
            Guid? newProjectId = null,
            Guid? newOwnerId = null,
            bool? newShowTabs = null,
            bool? newRecentlyViewedFlag = null,
            bool? newEncryptExtracts = null)
        {
            Workbook = new(newShowTabs, newRecentlyViewedFlag, newEncryptExtracts);

            if (newName is not null)
                Workbook.Name = newName;

            if (newDescription is not null)
                Workbook.Description = newDescription;

            if (newProjectId is not null)
                Workbook.Project = new() { Id = newProjectId.Value };

            if (newOwnerId is not null)
                Workbook.Owner = new() { Id = newOwnerId.Value };
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
            private bool? _showTabs;
            private bool? _recentlyViewed;
            private bool? _encryptExtracts;

            /// <summary>
            /// Default parameterless constructor.
            /// </summary>
            public WorkbookType() { }

            /// <summary>
            /// Creates a new <see cref="WorkbookType"/> instance.
            /// </summary>
            /// <param name="showTabs">Whether or not to show workbook views in tabs, or null to not update the option.</param>
            /// <param name="recentlyViewed">Whether or not to show the workbook in the recently viewed list, or null to not update the flag.</param>
            /// <param name="encryptExtracts">Whether or not to encrypt extracts, or null to not update the option.</param>
            public WorkbookType(
                bool? showTabs,
                bool? recentlyViewed,
                bool? encryptExtracts)
            {
                _showTabs = showTabs;
                _recentlyViewed = recentlyViewed;
                _encryptExtracts = encryptExtracts;
            }

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
            /// Gets or sets the project for the request.
            /// </summary>
            [XmlElement("project")]
            public ProjectType? Project { get; set; }

            /// <summary>
            /// Gets or sets the owner for the request.
            /// </summary>
            [XmlElement("owner")]
            public OwnerType? Owner { get; set; }

            /// <summary>
            /// Gets the show tabs option for the request.
            /// </summary>
            [XmlAttribute("showTabs")]
            public bool ShowTabs
            {
                get => _showTabs!.Value;
                set => _showTabs = value;
            }

            /// <summary>
            /// Defines the serialization for the property <see cref="ShowTabs"/>.
            /// </summary>
            [XmlIgnore]
            public bool ShowTabsSpecified => _showTabs.HasValue;

            /// <summary>
            /// Gets the recently viewed flag for the request.
            /// </summary>
            [XmlAttribute("recentlyViewed")]
            public bool RecentlyViewed
            {
                get => _recentlyViewed!.Value;
                set => _recentlyViewed = value;
            }

            /// <summary>
            /// Defines the serialization for the property <see cref="RecentlyViewed"/>.
            /// </summary>
            [XmlIgnore]
            public bool RecentlyViewedSpecified => _recentlyViewed.HasValue;

            /// <summary>
            /// Gets the encrypt extracts flag for the request.
            /// </summary>
            [XmlAttribute("encryptExtracts")]
            public bool EncryptExtracts
            {
                get => _encryptExtracts!.Value;
                set => _encryptExtracts = value;
            }

            /// <summary>
            /// Defines the serialization for the property <see cref="EncryptExtracts"/>.
            /// </summary>
            [XmlIgnore]
            public bool EncryptExtractsSpecified => _encryptExtracts.HasValue;
        }
    }
}
