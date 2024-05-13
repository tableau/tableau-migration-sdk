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

using System.Threading.Tasks;

namespace Tableau.Migration.Engine.Hooks.Mappings
{
    /// <summary>
    /// Context for <see cref="IContentMapping{TContent}"/> operations
    /// mapping a content item to an intended destination location for 
    /// publishing and content references.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    public class ContentMappingContext<TContent>
    {
        /// <summary>
        /// Gets the content item being mapped.
        /// </summary>
        public TContent ContentItem { get; }

        /// <summary>
        /// Gets the destination location the content item will be mapped and/or published to.
        /// </summary>
        public ContentLocation MappedLocation { get; }

        /// <summary>
        /// Creates a new <see cref="ContentMappingContext{TContent}"/> object.
        /// </summary>
        /// <param name="contentItem">The content item being mapped.</param>
        /// <param name="mappedLocation">The destination location to map to.</param>
        public ContentMappingContext(TContent contentItem, ContentLocation mappedLocation)
        {
            ContentItem = contentItem;
            MappedLocation = mappedLocation;
        }

        /// <summary>
        /// Maps the content item to a new destination location.
        /// </summary>
        /// <param name="mappedLocation">The destination location to map to.</param>
        /// <returns>A new context for the content item with the mapped location.</returns>
        public ContentMappingContext<TContent> MapTo(ContentLocation mappedLocation)
            => new(ContentItem, mappedLocation);

        /// <summary>
        /// Creates a task that's successfully completed from the current context.
        /// </summary>
        /// <returns>The successfully completed task.</returns>
        public Task<ContentMappingContext<TContent>?> ToTask()
            => Task.FromResult<ContentMappingContext<TContent>?>(this);
    }
}
