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
using Tableau.Migration.Content;

namespace Tableau.Migration.Api.Rest.Models.Requests
{
    /// <summary>    
    /// Class representing an add favorite request.
    /// </summary>
    [XmlType(XmlTypeName)]
    public class AddFavoriteRequest : TableauServerRequest
    {
        /// <summary>
        /// Create a new <see cref="AddFavoriteRequest"/> object.
        /// </summary>
        public AddFavoriteRequest()
        { }

        /// <summary>
        /// Create a new <see cref="AddFavoriteRequest"/> object.
        /// </summary>
        public AddFavoriteRequest(string label, FavoriteContentType contentType, Guid contentItemId)
        {
            Favorite = new() { Label = label };

            switch (contentType)
            {
                case FavoriteContentType.DataSource:
                    Favorite.DataSource = new(contentItemId);
                    break;
                case FavoriteContentType.Flow:
                    Favorite.Flow = new(contentItemId);
                    break;
                case FavoriteContentType.Project:
                    Favorite.Project = new(contentItemId);
                    break;
                case FavoriteContentType.View:
                    Favorite.View = new(contentItemId);
                    break;
                case FavoriteContentType.Workbook:
                    Favorite.Workbook = new(contentItemId);
                    break;
                case FavoriteContentType.Collection:
                    Favorite.Collection = new(contentItemId);
                    break;
                case FavoriteContentType.Unknown:
                    throw new ArgumentException("Cannot add a favorite with an unknown content type.");
                default:
                    break;
            }
        }

        /// <summary>
        /// Gets or sets the favorite for the request.
        /// </summary>
        [XmlElement("favorite")]
        public FavoriteType? Favorite { get; set; }

        /// <summary>
        /// The favorite type in the API request body.
        /// </summary>
        public class FavoriteType : IFavoriteType
        {
            /// <summary>
            /// Gets or sets the label for the favorite.
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

                /// <summary>
                /// Creates a new <see cref="DataSourceType"/> object.
                /// </summary>
                public DataSourceType()
                { }

                /// <summary>
                /// Creates a new <see cref="DataSourceType"/> object.
                /// </summary>
                /// <param name="id">The id.</param>
                public DataSourceType(Guid id)
                {
                    Id = id;
                }
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
                /// Creates a new <see cref="FlowType"/> object.
                /// </summary>
                public FlowType()
                { }

                /// <summary>
                /// Creates a new <see cref="FlowType"/> object.
                /// </summary>
                /// <param name="id">The id.</param>
                public FlowType(Guid id)
                {
                    Id = id;
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

                /// <summary>
                /// Creates a new <see cref="ProjectType"/> object.
                /// </summary>
                public ProjectType()
                { }

                /// <summary>
                /// Creates a new <see cref="ProjectType"/> object.
                /// </summary>
                /// <param name="id">The id.</param>
                public ProjectType(Guid id)
                {
                    Id = id;
                }
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
                /// Creates a new <see cref="ViewType"/> object.
                /// </summary>
                public ViewType()
                { }

                /// <summary>
                /// Creates a new <see cref="ViewType"/> object.
                /// </summary>
                /// <param name="id">The id.</param>
                public ViewType(Guid id)
                {
                    Id = id;
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

                /// <summary>
                /// Creates a new <see cref="WorkbookType"/> object.
                /// </summary>
                public WorkbookType()
                { }

                /// <summary>
                /// Creates a new <see cref="WorkbookType"/> object.
                /// </summary>
                /// <param name="id">The id.</param>
                public WorkbookType(Guid id)
                {
                    Id = id;
                }
            }

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
                /// Creates a new <see cref="CollectionType"/> object.
                /// </summary>
                public CollectionType()
                { }

                /// <summary>
                /// Creates a new <see cref="CollectionType"/> object.
                /// </summary>
                /// <param name="id">The id.</param>
                public CollectionType(Guid id)
                {
                    Id = id;
                }
            }
        }
    }
}
