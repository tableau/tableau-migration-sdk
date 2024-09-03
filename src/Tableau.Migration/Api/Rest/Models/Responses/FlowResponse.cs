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

using System.Linq;
using System;
using System.Xml.Serialization;

namespace Tableau.Migration.Api.Rest.Models.Responses
{
    /// <summary>
    /// Class representing a prep flow response.
    ///<see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_flow.htm#publish_flow">Tableau API Reference</see> for documentation.
    /// </summary>
    [XmlType(XmlTypeName)]
    public class FlowResponse : TableauServerResponse<FlowResponse.FlowType>
    {
        /// <summary>
        /// Gets or sets the prep flow for the response.
        /// </summary>
        [XmlElement("flow")]
        public override FlowType? Item { get; set; }

        /// <summary>
        /// Class representing a site response.
        /// </summary>
        public class FlowType : IFlowType
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
            /// Gets or sets the description for the response.
            /// </summary>
            [XmlAttribute("description")]
            public string? Description { get; set; }

            /// <summary>
            /// Gets or sets the webpage URL for the response.
            /// </summary>
            [XmlAttribute("webpageUrl")]
            public string? WebpageUrl { get; set; }

            /// <summary>
            /// Gets or sets the file type for the response.
            /// </summary>
            [XmlAttribute("fileType")]
            public string? FileType { get; set; }

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
            /// Gets or sets the project for the response.
            /// </summary>
            [XmlElement("project")]
            public ProjectType? Project { get; set; }

            IProjectReferenceType? IWithProjectType.Project => Project;

            /// <summary>
            /// Gets or sets the owner for the response.
            /// </summary>
            [XmlElement("owner")]
            public OwnerType? Owner { get; set; }

            IRestIdentifiable? IWithOwnerType.Owner => Owner;

            /// <summary>
            /// Gets or sets the tags for the response.
            /// </summary>
            [XmlArray("tags")]
            [XmlArrayItem("tag")]
            public TagType[] Tags { get; set; } = Array.Empty<TagType>();

            ITagType[] IWithTagTypes.Tags
            {
                get => Tags;
                set => Tags = value.Select(t => new TagType(t)).ToArray();
            }

            /// <summary>
            /// Class representing a REST API project response.
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
            }

            /// <summary>
            /// Class representing a REST API user response.
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

            /// <summary>
            /// Class representing a REST API tag response.
            /// </summary>
            public class TagType : ITagType
            {
                /// <summary>
                /// Gets or sets the label for the response.
                /// </summary>
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
        }
    }
}
