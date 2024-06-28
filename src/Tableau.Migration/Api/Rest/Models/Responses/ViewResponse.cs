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
    /// Class representing a REST API view response.
    /// </summary>
    [XmlType(XmlTypeName)]
    public class ViewResponse : TableauServerResponse<ViewResponse.ViewType>
    {
        /// <summary>
        /// Gets or sets the view for the response.
        /// </summary>
        [XmlElement("view")]
        public override ViewType? Item { get; set; }

        /// <summary>
        /// Class representing a REST API workbook response.
        /// </summary>
        public class ViewType : IViewType
        {
            /// <summary>
            /// Creates a new <see cref="ViewType"/> object.
            /// </summary>
            public ViewType() //Default constructor for serialization.
            { }

            ///<inheritdoc/>
            [XmlAttribute("id")]
            public Guid Id { get; set; }

            ///<inheritdoc/>
            [XmlAttribute("name")]
            public string? Name { get; set; }

            ///<inheritdoc/>
            [XmlAttribute("contentUrl")]
            public string? ContentUrl { get; set; }

            ///<inheritdoc/>
            [XmlAttribute("viewNameUrl")]
            public string? ViewUrlName { get; set; }

            ///<inheritdoc/>
            [XmlAttribute("createdAt")]
            public string? CreatedAt { get; set; }

            ///<inheritdoc/>
            [XmlAttribute("updatedAt")]
            public string? UpdatedAt { get; set; }

            ///<inheritdoc/>
            [XmlElement("workbook")]
            WorkbookReferenceType? Workbook { get; set; }

            IRestIdentifiable? IWithWorkbookReferenceType.Workbook => Workbook;

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
            /// Class representing a REST API workbook reference response.
            /// </summary>

            public class WorkbookReferenceType : IRestIdentifiable
            {
                /// <inheritdoc/>
				[XmlAttribute("label")]
                public Guid Id { get; set; }
            }

            #endregion
        }
    }
}
