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
    /// Class representing a custom views response.
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_workbooks_and_views.htm#list_custom_views">Tableau API Reference</see> for documentation.
    /// </summary>
    [XmlType(XmlTypeName)]
    public class CustomViewsResponse : PagedTableauServerResponse<CustomViewsResponse.CustomViewResponseType>
    {
        /// <summary>
        /// Gets or sets the custom views for the response.
        /// </summary>
        [XmlArray("customViews")]
        [XmlArrayItem("customView")]
        public override CustomViewResponseType[] Items { get; set; } = Array.Empty<CustomViewResponseType>();

        /// <summary>
        /// Class representing a custom view on the response.
        /// </summary>
        public class CustomViewResponseType : ICustomViewType
        {
            /// <summary>
            /// The default parameterless constructor.
            /// </summary>
            public CustomViewResponseType() { }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="customView"></param>
            public CustomViewResponseType(ICustomViewType customView)
            {
                Id = customView.Id;
                Name = customView.Name;
                CreatedAt = customView.CreatedAt;
                UpdatedAt = customView.UpdatedAt;
                LastAccessedAt = customView.LastAccessedAt;
                Shared = customView.Shared;
                View = new ViewType()
                {
                    Id = Guard.AgainstNull(customView.ViewId, () => customView.ViewId),
                    Name = customView.ViewName
                };

                Guard.AgainstNull(customView.Workbook, () => customView.Workbook);

                Workbook = new WorkbookType()
                {
                    Id = customView.Workbook.Id
                };

                Guard.AgainstNull(customView.Owner, () => customView.Owner);
                Owner = new OwnerType()
                {
                    Id = customView.Owner.Id
                };

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
                /// The default parameterless constructor.
                /// </summary>
                public OwnerType()
                { }

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
