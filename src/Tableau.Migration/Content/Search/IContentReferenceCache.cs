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
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Content.Search
{
    /// <summary>
    /// Interface for an object that can efficiently cache <see cref="IContentReference"/> objects for a given endpoint and content type.
    /// </summary>
    /// <remarks>Implementations should be thread safe due to parallel migration processing.</remarks>
    public interface IContentReferenceCache
    {
        /// <summary>
        /// Finds the content reference item for a given endpoint location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The content reference, or null if no item was found.</returns>
        Task<IContentReference?> ForLocationAsync(ContentLocation location, CancellationToken cancel);

        /// <summary>
        /// Finds the content reference item for a given endpoint ID.
        /// </summary>
        /// <param name="id">The ID.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The content reference, or null if no item was found.</returns>
        Task<IContentReference?> ForIdAsync(Guid id, CancellationToken cancel);
    }
}
