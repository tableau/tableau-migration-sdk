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
using Tableau.Migration.Api.Models;
using Tableau.Migration.Content;

namespace Tableau.Migration.Api.Rest.Models.Requests
{
    /// <summary>
    /// <para>
    /// Class representing a update labels request.
    /// </para>
    /// <para>
    /// See https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_metadata.htm#update_label_on_assets 
    /// for documentation
    /// </para>
    /// </summary>
    [XmlType(XmlTypeName)]
    public class UpdateLabelsRequest : TableauServerRequest
    {
        /// <summary>
        /// The default parameterless constructor.
        /// </summary>
        public UpdateLabelsRequest()
        { }

        /// <summary>
        /// Builds from content item type and content item ID.
        /// </summary>
        /// <param name="contentItemId">The ID of the content item to update the label for.</param>
        /// <param name="contentItemType">The label content type from <see cref="LabelContentTypes"/>.</param>
        /// <param name="labelUpdateOptions">The options for label to update or create.</param>  
        public UpdateLabelsRequest(Guid contentItemId, string contentItemType, ILabelUpdateOptions labelUpdateOptions)
        {
            ContentList = new[] { new ContentType(contentItemId, contentItemType) };
            Label = new LabelType(labelUpdateOptions);
        }


        /// <summary>
        /// The list of content items to get get labels for.
        /// </summary>        
        [XmlArray("contentList")]
        [XmlArrayItem("content")]
        public ContentType[]? ContentList { get; set; }

        /// <summary>
        /// The label to be updated.
        /// </summary>
        [XmlElement("label")]
        public LabelType? Label { get; set; }

        /// <summary>
        /// The class representing content in the request.
        /// </summary>        
        public class ContentType
        {
            /// <summary>
            /// The default parameterless constructor.
            /// </summary>
            public ContentType()
            { }

            /// <summary>
            /// Builds from content item type and content item ID.
            /// </summary>
            /// <param name="contentItemId">The ID of the content item to update the label for.</param>
            /// <param name="contentItemType">The label content type from <see cref="LabelContentTypes"/>.</param>   

            public ContentType(Guid contentItemId, string contentItemType)
            {
                ContentItemId = Guard.AgainstDefaultValue(contentItemId, () => contentItemId);
                ContentItemType = Guard.AgainstNullOrEmpty(contentItemType, () => contentItemType);
            }

            /// <summary>
            /// The ID of the content item to get labels for.
            /// </summary>
            [XmlAttribute("id")]
            public Guid ContentItemId { get; set; }

            /// <summary>
            /// The type of content item. It can be one of <see cref="LabelContentTypes"/>.
            /// </summary>
            [XmlAttribute("contentType")]
            public string? ContentItemType { get; set; }
        }

        /// <summary>
        /// The class representing the label in the request.
        /// </summary>
        public class LabelType
        {
            /// <summary>
            /// The default parameterless constructor.
            /// </summary>
            public LabelType()
            { }

            /// <summary>
            /// Builds from <see cref="ILabel"/>.
            /// </summary>
            /// <param name="updateOptions">The options for label to update or create.</param>
            public LabelType(ILabelUpdateOptions updateOptions)
            {
                Guard.AgainstNull(updateOptions, () => updateOptions);

                Value = Guard.AgainstNull(updateOptions.Value, () => updateOptions.Value);
                Message = updateOptions.Message;
                Active = updateOptions.Active;
                Elevated = updateOptions.Elevated;
            }

            /// <summary>
            /// Gets or sets the value.
            /// </summary>
            [XmlAttribute("value")]
            public string? Value { get; set; }

            /// <summary>
            /// Gets or sets the message.
            /// </summary>
            [XmlAttribute("message")]
            public string? Message { get; set; }

            /// <summary>
            /// Gets or sets the active flag.
            /// </summary>
            [XmlAttribute("active")]
            public bool Active { get; set; }

            /// <summary>
            /// Gets or sets the elevated flag.
            /// </summary>
            [XmlAttribute("elevated")]
            public bool Elevated { get; set; }
        }
    }
}
