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
    /// Class representing a paged REST API workbooks response.
    /// </summary>
    [XmlType(XmlTypeName)]
    public class WorkbooksResponse : PagedTableauServerResponse<WorkbooksResponse.WorkbookType>
    {
        /// <summary>
        /// Gets or sets the workbooks for the response.
        /// </summary>
        [XmlArray("workbooks")]
        [XmlArrayItem("workbook")]
        public override WorkbookType[] Items { get; set; } = Array.Empty<WorkbookType>();

        /// <summary>
        /// Class representing a REST API workbook response.
        /// </summary>
        public class WorkbookType : IWorkbookType
        {
            ///<inheritdoc/>
            [XmlAttribute("id")]
            public Guid Id { get; set; }

            ///<inheritdoc/>
            [XmlAttribute("name")]
            public string? Name { get; set; }

            ///<inheritdoc/>
            [XmlAttribute("description")]
            public string? Description { get; set; }

            ///<inheritdoc/>
            [XmlAttribute("contentUrl")]
            public string? ContentUrl { get; set; }

            ///<inheritdoc/>
            [XmlAttribute("showTabs")]
            public bool ShowTabs { get; set; }

            ///<inheritdoc/>
            [XmlAttribute("size")]
            public long Size { get; set; }

            ///<inheritdoc/>
            [XmlAttribute("webpageUrl")]
            public string? WebpageUrl { get; set; }

            ///<inheritdoc/>
            [XmlAttribute("createdAt")]
            public string? CreatedAt { get; set; }

            ///<inheritdoc/>
            [XmlAttribute("updatedAt")]
            public string? UpdatedAt { get; set; }

            ///<inheritdoc/>
            [XmlAttribute("defaultViewId")]
            public Guid DefaultViewId { get; set; }

            ///<inheritdoc/>
            [XmlAttribute("encryptExtracts")]
            public bool EncryptExtracts { get; set; }

            /// <summary>
            /// Gets or sets the project for the response.
            /// </summary>
            [XmlElement("project")]
            public ProjectType? Project { get; set; }

            ///<inheritdoc/>
            IProjectReferenceType? IWithProjectType.Project => Project;

            /// <summary>
            /// Gets or sets the location for the response.
            /// </summary>
            [XmlElement("location")]
            public LocationType? Location { get; set; }

            ///<inheritdoc/>
            ILocationType? IWorkbookType.Location => Location;

            /// <summary>
            /// Gets or sets the owner for the response.
            /// </summary>
            [XmlElement("owner")]
            public OwnerType? Owner { get; set; }

            ///<inheritdoc/>
            IRestIdentifiable? IWithOwnerType.Owner => Owner;

            /// <summary>
            /// Gets or sets the tags for the response.
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
            /// Class representing a REST API project response.
            /// </summary>
            public class ProjectType : IProjectReferenceType
            {
                /// <inheritdoc/>
                [XmlAttribute("id")]
                public Guid Id { get; set; }

                /// <inheritdoc/>
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

            /// <summary>
            /// Class representing a REST API location response.
            /// </summary>
            public class LocationType : ILocationType
            {
                /// <inheritdoc/>
                [XmlAttribute("id")]
                public Guid Id { get; set; }

                /// <inheritdoc/>
                [XmlAttribute("type")]
                public string? Type { get; set; }

                /// <inheritdoc/>
                [XmlAttribute("name")]
                public string? Name { get; set; }

                /// <summary>
                /// The default parameterless constructor.
                /// </summary>            
                public LocationType()
                { }

                /// <summary>
                /// Constructor to build from <see cref="ILocationType"/>.
                /// </summary>
                /// <param name="response">The <see cref="ILocationType"/> object.</param>
                public LocationType(ILocationType response)
                {
                    Id = response.Id;
                    Name = response.Name;
                }
            }

            /// <summary>
            /// Class representing a REST API user response.
            /// </summary>
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
                /// <param name="owner">The owner to copy from.</param>
                public OwnerType(IRestIdentifiable owner)
                {
                    Id = owner.Id;
                }
            }

            /// <summary>
            /// Class representing a REST API tag response.
            /// </summary>
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
            public WorkbookType()
            { }

            /// <summary>
            /// Constructor to build from <see cref="IWorkbookType"/>.
            /// </summary>
            /// <param name="response"></param>
            public WorkbookType(IWorkbookType response)
            {
                Id = response.Id;
                Name = response.Name;
                ContentUrl = response.ContentUrl;
                ShowTabs = response.ShowTabs;
                Size = response.Size;
                WebpageUrl = response.WebpageUrl;
                CreatedAt = response.CreatedAt;
                UpdatedAt = response.UpdatedAt;
                EncryptExtracts = response.EncryptExtracts;

                Project = response.Project is null ?
                    null :
                    new ProjectType(response.Project);

                if (response.Location is not null)
                {
                    Location = new LocationType(response.Location);
                }

                if (response.Owner is not null)
                {
                    Owner = new OwnerType(response.Owner);
                }

                Tags = response.Tags.IsNullOrEmpty() ?
                    Array.Empty<TagType>() :
                    response.Tags.Select(t => new TagType(t)).ToArray();

                Description = response.Description;
            }
        }
    }
}
