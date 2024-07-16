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
using Tableau.Migration.Content;

namespace Tableau.Migration.JsonConverters.SerializableObjects
{
    /// <summary>
    /// Represents a JSON serializable content reference.
    /// </summary>
    public class SerializableContentReference
    {
        /// <summary>
        /// Gets or sets the unique identifier for the content.
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// Gets or sets the URL associated with the content.
        /// </summary>
        public string? ContentUrl { get; set; }

        /// <summary>
        /// Gets or sets the location information for the content.
        /// </summary>
        public SerializableContentLocation? Location { get; set; }

        /// <summary>
        /// Gets or sets the name of the content.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableContentReference"/> class.
        /// </summary>
        public SerializableContentReference() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableContentReference"/> class with details from an <see cref="IContentReference"/>.
        /// </summary>
        /// <param name="content">The content reference to serialize.</param>
        internal SerializableContentReference(IContentReference content)
        {
            Id = content.Id.ToString();
            ContentUrl = content.ContentUrl;
            Location = new SerializableContentLocation(content.Location);
            Name = content.Name;
        }

        /// <summary>
        /// Throw exception if any values are still null
        /// </summary>
        public void VerifyDeserialization()
        {
            Guard.AgainstNull(Id, nameof(Id));
            Guard.AgainstNull(ContentUrl, nameof(ContentUrl));
            Guard.AgainstNull(Location, nameof(Location));
            Guard.AgainstNull(Name, nameof(Name));
        }

        /// <summary>
        /// Returns the current item as a <see cref="ContentReferenceStub"/>
        /// </summary>
        /// <returns></returns>
        public ContentReferenceStub AsContentReferenceStub()
        {
            VerifyDeserialization();

            var ret = new ContentReferenceStub(
                Guid.Parse(Id!),
                ContentUrl!,
                Location!.AsContentLocation(),
                Name!);

            return ret;
        }
    }
}