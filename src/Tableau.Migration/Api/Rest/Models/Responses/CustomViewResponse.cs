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
    /// Class representing a custom view response.
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_workbooks_and_views.htm#get_custom_view">Tableau API Reference</see> for documentation.
    /// </summary>
    [XmlType(XmlTypeName)]
    public class CustomViewResponse : TableauServerResponse<CustomViewResponse.CustomViewType>
    {
        /// <summary>
        /// Gets or sets the custom view for the response.
        /// </summary>
        [XmlElement("customView")]
        public override CustomViewType? Item { get; set; }

        /// <summary>
        /// Class representing a custom view on the response.
        /// </summary> 
        public class CustomViewType : ICustomViewType
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
            /// Gets or sets the createdAt timestamp for the response.
            /// </summary>
            [XmlAttribute("createdAt")]
            public string? CreatedAt { get; set; }

            /// <summary>
            /// Gets or sets the updatedAt timestamp for the response.
            /// </summary>
            [XmlAttribute("updatedAt")]
            public string? UpdatedAt { get; set; }

            /// <summary>
            /// Gets or sets the lastAccessedAt timestamp for the response.
            /// </summary>
            [XmlAttribute("lastAccessedAt")]
            public string? LastAccessedAt { get; set; }

            /// <summary>
            /// Gets or sets the shared flag for the response.
            /// </summary>
            [XmlAttribute("shared")]
            public bool Shared { get; set; }

            /// <summary>
            /// Gets or sets the view for the response.
            /// </summary>
            [XmlElement("view")]
            public ViewType? View { get; set; }

            Guid? ICustomViewType.ViewId => View?.Id;

            string? ICustomViewType.ViewName => View?.Name;

            /// <summary>
            /// Gets or sets the workbook for the response.
            /// </summary>
            [XmlElement("workbook")]
            public WorkbookType? Workbook { get; set; }

            IRestIdentifiable? IWithWorkbookReferenceType.Workbook => Workbook;

            /// <summary>
            /// Gets or sets the owner for the response.
            /// </summary>
            [XmlElement("owner")]
            public OwnerType? Owner { get; set; }

            IRestIdentifiable? IWithOwnerType.Owner => Owner;

            /// <summary>
            /// Class representing a REST API view on the response.
            /// </summary>       
            public class ViewType : IRestIdentifiable
            {
                /// <summary>
                /// The default parameterless constructor.
                /// </summary>
                public ViewType()
                { }

                /// <summary>
                /// The constructor to build from ID and Name.
                /// </summary>
                /// <param name="id">The ID of the base view.</param>
                /// <param name="name">The name of the base view.</param>
                public ViewType(Guid id, string name)
                {
                    Id = id;
                    Name = name;
                }

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
            /// Class representing a REST API workbook on the response.
            /// </summary>       
            public class WorkbookType : IRestIdentifiable
            {
                /// <summary>
                /// The default parameterless constructor.
                /// </summary>
                public WorkbookType()
                { }

                /// <summary>
                /// 
                /// </summary>
                /// <param name="workbook"></param>
                public WorkbookType(WorkbookResponse.WorkbookType workbook)
                {
                    Id = workbook.Id;
                    Name = workbook.Name;
                }

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
            /// Class representing a REST API owner on the response.
            /// </summary> 
            public class OwnerType : IRestIdentifiable
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
        }
    }
}
