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
    /// Class representing a paged REST API favorites response.
    /// </summary>
    [XmlType(XmlTypeName)]
    public class FavoritesResponse : PagedTableauServerResponse<FavoritesResponse.FavoriteType>
    {
        /// <summary>
        /// Gets or sets the workbooks for the response.
        /// </summary>
        [XmlArray("favorites")]
        [XmlArrayItem("favorite")]
        public override FavoriteType[] Items { get; set; } = Array.Empty<FavoriteType>();

        /// <summary>
        /// Class representing a REST API favorite response.
        /// </summary>
        public class FavoriteType : IFavoriteType
        {
            /// <summary>
            /// Gets the label of the favorite.
            /// </summary>
            [XmlAttribute("label")]
            public string? Label { get; set; }

            IRestIdentifiable? IFavoriteType.DataSource => DataSource;

            IRestIdentifiable? IFavoriteType.Flow => Flow;

            IRestIdentifiable? IFavoriteType.Project => Project;

            IRestIdentifiable? IFavoriteType.View => View;

            IRestIdentifiable? IFavoriteType.Workbook => Workbook;

            IRestIdentifiable? IFavoriteType.Collection => Collection;

            /// <summary>
            /// Gets the collection of the favorite.
            /// </summary>
            [XmlElement("collection")]
            public CollectionType? Collection { get; set; }

            /// <summary>
            /// Class representing a REST API favorite response's collection.
            /// </summary>
            public class CollectionType : IRestIdentifiable
            {
                /// <inheritdoc/>
                [XmlAttribute("id")]
                public Guid Id { get; set; }

                /// <summary>
                /// Gets or sets the name of the collection.
                /// </summary>
                [XmlAttribute("name")]
                public string? Name { get; set; }

                /// <summary>
                /// Gets or sets the creation date of the collection.
                /// </summary>
                [XmlAttribute("createdAt")]
                public DateTime CreatedAt { get; set; }

                /// <summary>
                /// Gets or sets the last update date of the collection.
                /// </summary>
                [XmlAttribute("updatedAt")]
                public DateTime UpdatedAt { get; set; }

                /// <summary>
                /// Gets or sets the description of the collection.
                /// </summary>
                [XmlAttribute("description")]
                public string? Description { get; set; }

                /// <summary>
                /// Gets or sets the count of permissioned items in the collection.
                /// </summary>
                [XmlAttribute("permissionedItemCount")]
                public int PermissionedItemCount { get; set; }

                /// <summary>
                /// Gets or sets the total count of items in the collection.
                /// </summary>
                [XmlAttribute("totalItemCount")]
                public int TotalItemCount { get; set; }

                /// <summary>
                /// Gets or sets the visibility of the collection.
                /// </summary>
                [XmlAttribute("visibility")]
                public string? Visibility { get; set; }

                /// <summary>
                /// Gets or sets the owner of the collection.
                /// </summary>
                [XmlElement("owner")]
                public OwnerType? Owner { get; set; }

                /// <summary>
                /// Class representing a REST API collection response's owner.
                /// </summary>
                public class OwnerType : IRestIdentifiable
                {
                    /// <inheritdoc/>
                    [XmlAttribute("id")]
                    public Guid Id { get; set; }
                }
            }

            /// <summary>
            /// Gets the data source of the favorite.
            /// </summary>
            [XmlElement("datasource")]
            public DataSourceType? DataSource { get; set; }

            /// <summary>
            /// Class representing a REST API favorite response's data source.
            /// </summary>
            public class DataSourceType : IRestIdentifiable
            {
                /// <inheritdoc/>
                [XmlAttribute("id")]
                public Guid Id { get; set; }
            }

            /// <summary>
            /// Gets the flow of the favorite.
            /// </summary>
            [XmlElement("flow")]
            public FlowType? Flow { get; set; }

            /// <summary>
            /// Class representing a REST API favorite response's flow.
            /// </summary>
            public class FlowType : IRestIdentifiable
            {
                /// <inheritdoc/>
                [XmlAttribute("id")]
                public Guid Id { get; set; }

                /// <summary>
                /// Gets the name of the view.
                /// </summary>
                [XmlElement("name")]
                public string? Name { get; set; }

                /// <summary>
                /// Gets the project of the flow.
                /// </summary>
                [XmlElement("project")]
                public FlowProjectType? Project { get; set; }

                /// <summary>
                /// Class representing a REST API flow response's project.
                /// </summary>
                public class FlowProjectType : IRestIdentifiable
                {
                    /// <inheritdoc/>
                    [XmlAttribute("id")]
                    public Guid Id { get; set; }
                }
            }

            /// <summary>
            /// Gets the project of the favorite.
            /// </summary>
            [XmlElement("project")]
            public ProjectType? Project { get; set; }

            /// <summary>
            /// Class representing a REST API favorite response's project.
            /// </summary>
            public class ProjectType : IRestIdentifiable
            {
                /// <inheritdoc/>
                [XmlAttribute("id")]
                public Guid Id { get; set; }
            }

            /// <summary>
            /// Gets the view of the favorite.
            /// </summary>
            [XmlElement("view")]
            public ViewType? View { get; set; }

            /// <summary>
            /// Class representing a REST API favorite response's view.
            /// </summary>
            public class ViewType : IRestIdentifiable
            {
                /// <inheritdoc/>
                [XmlAttribute("id")]
                public Guid Id { get; set; }

                /// <summary>
                /// Gets the name of the view.
                /// </summary>
                [XmlElement("name")]
                public string? Name { get; set; }

                /// <summary>
                /// Gets the content URL of the view.
                /// </summary>
                [XmlElement("contentUrl")]
                public string? ContentUrl { get; set; }

                /// <summary>
                /// Gets the workbook of the view.
                /// </summary>
                [XmlElement("workbook")]
                public ViewWorkbookType? Workbook { get; set; }

                /// <summary>
                /// Class representing a REST API view response's workbook.
                /// </summary>
                public class ViewWorkbookType : IRestIdentifiable
                {
                    /// <inheritdoc/>
                    [XmlAttribute("id")]
                    public Guid Id { get; set; }
                }
            }

            /// <summary>
            /// Gets the workbook of the favorite.
            /// </summary>
            [XmlElement("workbook")]
            public WorkbookType? Workbook { get; set; }

            /// <summary>
            /// Class representing a REST API favorite response's workbook.
            /// </summary>
            public class WorkbookType : IRestIdentifiable
            {
                /// <inheritdoc/>
                [XmlAttribute("id")]
                public Guid Id { get; set; }
            }
        }
    }
}
