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
using System.Linq;
using System.Xml.Serialization;

namespace Tableau.Migration.Api.Rest.Models.Responses
{
    /// <summary>
    /// Class representing a REST API workbook response.
    /// </summary>
    [XmlType(XmlTypeName)]
    public class WorkbookResponse : TableauServerResponse<WorkbookResponse.WorkbookType>
    {
        /// <summary>
        /// Gets or sets the workbook for the response.
        /// </summary>
        [XmlElement("workbook")]
        public override WorkbookType? Item { get; set; }

        /// <summary>
        /// Class representing a REST API workbook response.
        /// </summary>
        public class WorkbookType : IWorkbookDetailsType
        {
            /// <summary>
            /// Creates a new <see cref="WorkbookType"/> object.
            /// </summary>
            public WorkbookType() //Default constructor for serialization.
            { }

            internal WorkbookType(IWorkbookType response)
            {
                Id = response.Id;
                Name = response.Name;
                Description = response.Description;
                ContentUrl = response.ContentUrl;
                ShowTabs = response.ShowTabs;
                Size = response.Size;
                CreatedAt = response.CreatedAt;
                UpdatedAt = response.UpdatedAt;
                EncryptExtracts = response.EncryptExtracts;
                WebpageUrl = response.WebpageUrl;
                DefaultViewId = response.DefaultViewId;

                if (response.Project is not null)
                {
                    Project = new ProjectType(response.Project);
                }

                if (response.Owner is not null)
                {
                    Owner = new OwnerType(response.Owner);
                }

                Tags = response.Tags.Select(tag => new TagType(tag)).ToArray();
            }

            internal WorkbookType(IWorkbookDetailsType response)
                : this((IWorkbookType)response)
            { }

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
            IProjectNamedReferenceType? IWithProjectNamedReferenceType.Project => Project;

            IProjectReferenceType? IWithProjectReferenceType.Project => Project;

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

            /// <summary>
            /// Gets or sets the views for the response.
            /// </summary>
            [XmlArray("views")]
            [XmlArrayItem("view")]
            public WorkbookViewReferenceType[] Views { get; set; } = Array.Empty<WorkbookViewReferenceType>();

            ///<inheritdoc/>
            IWorkbookViewReferenceType[] IWorkbookDetailsType.Views => Views;

            /// <summary>
            /// Gets or sets the data acceleration config for the response.
            /// </summary>
            [XmlElement("dataAccelerationConfig")]
            public DataAccelerationConfigType? DataAccelerationConfig { get; set; }

            #region - Object Specific Types -

            /// <summary>
            /// Class representing a REST API project response.
            /// </summary>
            public class ProjectType : IProjectNamedReferenceType
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
                /// Constructor to build from <see cref="IProjectNamedReferenceType"/>.
                /// </summary>
                /// <param name="project">The <see cref="IProjectNamedReferenceType"/> object.</param>
                public ProjectType(IProjectNamedReferenceType project)
                {
                    Id = project.Id;
                    Name = project.Name;
                }

                /// <summary>
                /// Constructor to build from <see cref="IProjectType"/>.
                /// </summary>
                /// <param name="project">The <see cref="IProjectType"/> object.</param>
                public ProjectType(IProjectType project)
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

            /// <summary>
            /// Class representing a REST API view response.
            /// </summary>
            public class WorkbookViewReferenceType : IWorkbookViewReferenceType
            {
                /// <summary>
                /// The default parameterless constructor.
                /// </summary>
                public WorkbookViewReferenceType()
                { }

                /// <summary>
                /// Constructor to build from <see cref="IWorkbookViewReferenceType"/>.
                /// </summary>
                /// <param name="view">The <see cref="IWorkbookViewReferenceType"/> object.</param>
                public WorkbookViewReferenceType(IWorkbookViewReferenceType view)
                {
                    Id = view.Id;
                    Name = view.Name;
                    ContentUrl = view.ContentUrl;
                    Tags = view.Tags.Select(tag => new ViewTagType(tag)).ToArray();
                }

                /// <summary>
                /// Constructor to build from <see cref="IViewType"/>.
                /// </summary>
                /// <param name="view">The <see cref="IViewType"/> object.</param>
                public WorkbookViewReferenceType(IViewType view)
                {
                    Id = view.Id;
                    Name = view.Name;
                    ContentUrl = view.ContentUrl;
                    Tags = view.Tags.Select(tag => new ViewTagType(tag)).ToArray();
                }

                ///<inheritdoc/>
                [XmlAttribute("id")]
                public Guid Id { get; set; }

                ///<inheritdoc/>
                [XmlAttribute("name")]
                public string? Name { get; set; } = string.Empty;

                ///<inheritdoc/>
                [XmlAttribute("contentUrl")]
                public string? ContentUrl { get; set; } = string.Empty;

                /// <summary>
                /// Gets or sets the tags for the response.
                /// </summary>
                [XmlArray("tags")]
                [XmlArrayItem("tag")]
                public ViewTagType[] Tags { get; set; } = Array.Empty<ViewTagType>();

                ITagType[] IWithTagTypes.Tags
                {
                    get => Tags;
                    set => Tags = value.Select(t => new ViewTagType(t)).ToArray();
                }

                /// <summary>
                /// Class representing a REST API view tags response.
                /// </summary>
                public class ViewTagType : ITagType
                {
                    /// <summary>
                    /// The default parameterless constructor.
                    /// </summary>
                    public ViewTagType()
                    { }

                    /// <summary>
                    /// Constructor to build from <see cref="ITagType"/>
                    /// </summary>
                    public ViewTagType(ITagType tag)
                    {
                        Label = tag.Label;
                    }

                    /// <inheritdoc/>
                    [XmlAttribute("label")]
                    public string? Label { get; set; }
                }
            }
        }

        /// <summary>
        /// Class representing a REST API data acceleration config response.
        /// </summary>
        public class DataAccelerationConfigType
        {
            /// <summary>
            /// Gets or sets the acceleration enabled value for the response.
            /// </summary>
            [XmlAttribute("accelerationEnabled")]
            public bool AccelerationEnabled { get; set; }
        }

        #endregion
    }
}
