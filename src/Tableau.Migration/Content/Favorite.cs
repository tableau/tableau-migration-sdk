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

namespace Tableau.Migration.Content
{
    internal class Favorite : IFavorite
    {
        /// <inheritdoc/>
        public string Label { get; set; }

        /// <inheritdoc/>
        public IContentReference User { get; set; }

        /// <inheritdoc/>
        public IContentReference Content { get; set; }

        /// <inheritdoc/>
        public FavoriteContentType ContentType { get; }

        #region - IContentReference Implementation - 

        /// <inheritdoc/>
        public string ContentUrl { get; } = string.Empty;

        /// <inheritdoc/>
        public ContentLocation Location { get; }

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public Guid Id { get; } = Guid.Empty;

        #endregion

        public Favorite(IContentReference user, FavoriteContentType contentType, IContentReference contentTypeReference, string? label)
        {
            Name = Label = Guard.AgainstNullEmptyOrWhiteSpace(label, () => label);
            User = user;
            Content = contentTypeReference;
            ContentType = contentType;
            Location = BuildLocation(user, contentType, contentTypeReference);
        }

        internal static ContentLocation BuildLocation(IContentReference user, FavoriteContentType contentType, IContentReference content)
            => BuildLocation(user.Location, contentType, content.Location);

        internal static ContentLocation BuildLocation(ContentLocation userLocation, FavoriteContentType contentType, ContentLocation contentLocation)
            => new(userLocation.PathSegments.Concat(["favorites", contentType.ToString()]).Concat(contentLocation.PathSegments));

        public bool Equals(IContentReference? other)
            => Location.Equals(other?.Location);

    }
}

