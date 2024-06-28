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
using Tableau.Migration.Api.Rest;

namespace Tableau.Migration
{
    /// <summary>
    /// Interface for an object that describes information on how to reference an item of content,
    /// for example through a Tableau API.
    /// </summary>
    public interface IContentReference : IEquatable<IContentReference>, IRestIdentifiable
    {
        /// <summary>
        /// Get the site-unique "content URL" of the content item, 
        /// or an empty string if the content type does not use a content URL.
        /// </summary>
        string ContentUrl { get; }

        /// <summary>
        /// Gets the logical location path of the content item,
        /// for project-level content this is the project path and the content item name.
        /// </summary>
        ContentLocation Location { get; }

        /// <summary>
        /// Gets the name of the content item.
        /// This is equivalent to the last segment of the <see cref="Location"/>.
        /// Renames should be performed through mapping.
        /// </summary>
        string Name { get; }
    }
}
