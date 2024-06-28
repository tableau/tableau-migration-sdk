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
    /// <see cref="IContentReferenceFinder{TContent}"/> implementation that uses 
    /// a <see cref="IContentReferenceCache"/> to find content references.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    /// <remarks>
    /// Creates a new <see cref="CachedContentReferenceFinder{TContent}"/> object.
    /// </remarks>
    /// <param name="cache">The content reference cache.</param>
    public class CachedContentReferenceFinder<TContent>(IContentReferenceCache cache) : IContentReferenceFinder<TContent>
        where TContent : IContentReference
    {
        /// <inheritdoc />
        public async Task<IContentReference?> FindByIdAsync(Guid id, CancellationToken cancel)
            => await cache.ForIdAsync(id, cancel).ConfigureAwait(false);
    }
}
