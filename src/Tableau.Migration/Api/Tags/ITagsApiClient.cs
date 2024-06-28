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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Content;

namespace Tableau.Migration.Api.Tags
{
    /// <summary>
    /// Interface for an API client that modifies content's tags.
    /// </summary>
    public interface ITagsApiClient
    {
        /// <summary>
        /// Adds tags to the content item.
        /// </summary>
        /// <param name="contentItemId">The ID of the content item.</param>
        /// <param name="tags">The tags to add.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>All tags on content item.</returns>
        Task<IResult<IImmutableList<ITag>>> AddTagsAsync(Guid contentItemId, IEnumerable<ITag> tags, CancellationToken cancel);


        /// <summary>
        /// Remove tags from the content item.
        /// </summary>
        /// <param name="contentItemId">The ID of the content item.</param>
        /// <param name="tags">The tags to remove.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns></returns>
        Task<IResult> RemoveTagsAsync(Guid contentItemId, IEnumerable<ITag> tags, CancellationToken cancel);

        /// <summary>
        /// Adds and removes tags from the content item to match the new tags.
        /// </summary>
        /// <param name="contentItemId">The ID of the content ite.</param>
        /// <param name="tags">The tags to update to match.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns></returns>
        Task<IResult> UpdateTagsAsync(Guid contentItemId, IEnumerable<ITag> tags, CancellationToken cancel);
    }
}