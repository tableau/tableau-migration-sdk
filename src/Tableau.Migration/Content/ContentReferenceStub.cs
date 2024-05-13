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
using Tableau.Migration.Content.Search;

namespace Tableau.Migration.Content
{
    /// <summary>
    /// <see cref="IContentReference"/> implementation that only contains content reference fields, 
    /// used for <see cref="IContentReferenceCache"/> to avoid storing full content items.
    /// </summary>
    public sealed class ContentReferenceStub : IContentReference
    {
        /// <inheritdoc/>
        public Guid Id { get; init; }

        /// <inheritdoc/>
        public string ContentUrl { get; init; }

        /// <inheritdoc/>
        public ContentLocation Location { get; init; }

        /// <inheritdoc/>
        public string Name { get; init; }

        /// <summary>
        /// Creates a new <see cref="ContentReferenceStub"/> object.
        /// </summary>
        /// <param name="copy">An object to copy values from.</param>
        public ContentReferenceStub(IContentReference copy)
        {
            Id = copy.Id;
            ContentUrl = copy.ContentUrl;
            Location = copy.Location;
            Name = copy.Name;
        }

        /// <summary>
        /// Creates a new <see cref="ContentReferenceStub"/> object.
        /// </summary>
        /// <param name="id">
        /// The unique identifier of the content item, 
        /// corresponding to the LUID in the Tableau REST API.
        /// </param>
        /// <param name="contentUrl">
        /// The site-unique "content URL" of the content item, 
        /// or an empty string if the content type does not use a content URL.
        /// </param>
        /// <param name="location">
        /// The logical location path of the content item,
        /// for project-level content this is the project path and the content item name.
        /// </param>
        public ContentReferenceStub(Guid id, string contentUrl, ContentLocation location)
        {
            Id = id;
            ContentUrl = contentUrl;
            Location = location;
            Name = location.Name;
        }

        /// <summary>
        /// Creates a new <see cref="ContentReferenceStub"/> object.
        /// </summary>
        /// <param name="id">
        /// The unique identifier of the content item, 
        /// corresponding to the LUID in the Tableau REST API.
        /// </param>
        /// <param name="contentUrl">
        /// The site-unique "content URL" of the content item, 
        /// or an empty string if the content type does not use a content URL.
        /// </param>
        /// <param name="location">
        /// The logical location path of the content item,
        /// for project-level content this is the project path and the content item name.
        /// </param>
        /// /// <param name="name">
        /// The name of the content item
        /// </param>
        public ContentReferenceStub(Guid id, string contentUrl, ContentLocation location, string name)
        {
            Id = id;
            ContentUrl = contentUrl;
            Location = location;
            Name = name;
        }

        /// <inheritdoc/>
        public bool Equals(IContentReference? other)
        {
            if (other == null && GetType() != other!.GetType())
                return false;

            var ret = Id.Equals(other.Id) && (ContentUrl.CompareTo(other.ContentUrl) == 0) && Location.Equals(other.Location) && Name.Equals(other.Name);
            return ret;
        }

        /// <inheritdoc/>
        public override bool Equals(object? otherObj)
        {
            if (otherObj == null && GetType() != otherObj!.GetType())
                return false;

            var otherAsIContentReference = otherObj as IContentReference;
            if (otherAsIContentReference is null)
                return false;
            else
                return this.Equals(otherAsIContentReference);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(Id, ContentUrl, Location, Name);
        }

        /// <inheritdoc/>
        public static bool operator ==(ContentReferenceStub? a, ContentReferenceStub? b)
        {
            if (a is null && b is null) return true;
            if (a is not null && b is null) return false;
            if (a is null & b is not null) return false;

            return a!.Equals(b);
        }

        /// <inheritdoc/>
        public static bool operator !=(ContentReferenceStub? a, ContentReferenceStub? b)
        {
            if (a is null && b is null) return false;
            if (a is not null && b is null) return true;
            if (a is null & b is not null) return true;

            return !(a!.Equals(b));
        }
    }
}
