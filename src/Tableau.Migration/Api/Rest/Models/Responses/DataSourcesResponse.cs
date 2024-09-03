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
using System.Linq;
using System.Xml.Serialization;

namespace Tableau.Migration.Api.Rest.Models.Responses
{
    /// <summary>
    /// Class representing a data source response.
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref.htm#query_data_sources">Tableau API Reference</see> for documentation.
    /// </summary>
    [XmlType(XmlTypeName)]
    public class DataSourcesResponse : PagedTableauServerResponse<DataSourcesResponse.DataSourceType>
    {
        /// <summary>
        /// Gets or sets the data sources for the response.
        /// </summary>
        [XmlArray("datasources")]
        [XmlArrayItem("datasource")]
        public override DataSourceType[] Items { get; set; } = Array.Empty<DataSourceType>();

        /// <summary>
        /// Class representing a data source on the response.
        /// </summary>
        [XmlType("datasource")]
        public class DataSourceType : IDataSourceType
        {
            /// <summary>
            /// Gets or sets the ID for the response.
            /// </summary>
            [XmlAttribute("id")]
            public Guid Id { get; set; }

            /// <summary>
            /// Gets or sets the description for the response.
            /// </summary>
            [XmlAttribute("description")]
            public string? Description { get; set; }

            /// <summary>
            /// Gets or sets the name for the response.
            /// </summary>
            [XmlAttribute("name")]
            public string? Name { get; set; }

            /// <summary>
            /// Gets or sets the content URL for the response.
            /// </summary>
            [XmlAttribute("contentUrl")]
            public string? ContentUrl { get; set; }

            /// <summary>
            /// Gets or sets the created timestamp for the response.
            /// </summary>
            [XmlAttribute("createdAt")]
            public string? CreatedAt { get; set; }

            /// <summary>
            /// Gets or sets the updated timestamp for the response.
            /// </summary>
            [XmlAttribute("updatedAt")]
            public string? UpdatedAt { get; set; }

            /// <summary>
            /// Gets or sets the encrypted extracts flag for the response.
            /// </summary>
            [XmlAttribute("encryptExtracts")]
            public bool EncryptExtracts { get; set; }

            /// <summary>
            /// Gets or sets whether or not the data source has extracts for the response.
            /// </summary>
            [XmlAttribute("hasExtracts")]
            public bool HasExtracts { get; set; }

            /// <summary>
            /// Gets or sets whether or not the data source is certified for the response.
            /// </summary>
            [XmlAttribute("isCertified")]
            public bool IsCertified { get; set; }

            /// <summary>
            /// Gets or sets whether or not the data source uses a remote query agent for the response.
            /// </summary>
            [XmlAttribute("useRemoteQueryAgent")]
            public bool UseRemoteQueryAgent { get; set; }

            /// <summary>
            /// Gets or sets the data source webpage URL for the response.
            /// </summary>
            [XmlAttribute("webpageUrl")]
            public string? WebpageUrl { get; set; }

            /// <summary>
            /// Gets or sets the data source project for the response.
            /// </summary>
            [XmlElement("project")]
            public ProjectType? Project { get; set; }

            IProjectReferenceType? IWithProjectType.Project => Project;

            /// <summary>
            /// Gets or sets the data source owner for the response.
            /// </summary>
            [XmlElement("owner")]
            public OwnerType? Owner { get; set; }

            IRestIdentifiable? IWithOwnerType.Owner => Owner;

            /// <summary>
            /// Gets or sets the data source tags for the response.
            /// </summary>
            [XmlArray("tags")]
            [XmlArrayItem("tag")]
            public TagType[] Tags { get; set; } = Array.Empty<TagType>();

            ///<inheritdoc/>
            ITagType[] IWithTagTypes.Tags
            {
                get => Tags;
                set => Tags = value.Select(t => new TagType(t)).ToArray();
            }

            #region - Object Specific Types -

            /// <summary>
            /// Class representing a project on the response.
            /// </summary>
            public class ProjectType : IProjectReferenceType
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
                /// The default parameterless constructor.
                /// </summary>            
                public ProjectType()
                { }

                /// <summary>
                /// Constructor to build from <see cref="IProjectReferenceType"/>.
                /// </summary>
                /// <param name="project">The <see cref="IProjectReferenceType"/> object.</param>
                public ProjectType(IProjectReferenceType project)
                {
                    Id = project.Id;
                    Name = project.Name;
                }
            }

            /// <inheritdoc/>
            public class OwnerType : IRestIdentifiable
            {
                /// <inheritdoc/>
                [XmlAttribute("id")]
                public Guid Id { get; set; }

                /// <summary>
                /// The default parameterless constructor.
                /// </summary>            
                public OwnerType()
                { }

                /// <summary>
                /// Constructor to build from <see cref="IRestIdentifiable"/>
                /// </summary>
                /// <param name="owner"></param>
                public OwnerType(IRestIdentifiable owner)
                {
                    Id = owner.Id;
                }
            }

            /// <inheritdoc/>
            public class TagType : ITagType
            {
                /// <inheritdoc/>
                [XmlAttribute("label")]
                public string? Label { get; set; }

                /// <summary>
                /// The default parameterless constructor.
                /// </summary>            
                public TagType()
                { }

                /// <summary>
                /// Constructor to build from <see cref="ITagType"/>
                /// </summary>
                /// <param name="tag">The <see cref="ITagType"/> object.</param>
                public TagType(ITagType tag)
                {
                    Label = tag.Label;
                }
            }

            #endregion

            /// <summary>
            /// The default parameterless constructor.
            /// </summary>
            public DataSourceType()
            { }

            /// <summary>
            /// Constructor to convert from from <see cref="DataSourceResponse.DataSourceType"/>.
            /// </summary>
            /// <param name="response"></param>
            public DataSourceType(IDataSourceType response)
            {
                Id = response.Id;
                Name = response.Name;
                Description = response.Description;
                ContentUrl = response.ContentUrl;
                CreatedAt = response.CreatedAt;
                UpdatedAt = response.UpdatedAt;
                EncryptExtracts = response.EncryptExtracts;
                HasExtracts = response.HasExtracts;
                IsCertified = response.IsCertified;
                UseRemoteQueryAgent = response.UseRemoteQueryAgent;
                WebpageUrl = response.WebpageUrl;

                if (response.Project is not null)
                {
                    Project = new ProjectType(response.Project);
                }

                if (response.Owner is not null)
                {
                    Owner = new OwnerType(response.Owner);
                }

                Tags = response.Tags.IsNullOrEmpty() ?
                    Array.Empty<TagType>() :
                    response.Tags.Select(tag => new TagType(tag)).ToArray();
            }
        }
    }
}
