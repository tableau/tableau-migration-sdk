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

namespace Tableau.Migration.Content
{
    /// <summary>
    /// Interface for a content item named favorite.
    /// </summary>
    public interface IFavorite : IEmptyIdContentReference
    {
        /// <summary>
        /// Gets or sets the label for the favorite.
        /// </summary>
        string Label { get; set; }

        /// <summary>
        /// Gets the user <see cref="IContentReference"/> for the favorite.
        /// </summary>
        IContentReference User { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IContentReference"/> for the favorite.
        /// </summary>
        IContentReference Content { get; set; }

        /// <summary>
        /// Gets or sets the content type for the favorite.
        /// </summary>
        FavoriteContentType ContentType { get; }
    }
}
