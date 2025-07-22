﻿//
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
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Content.Search
{
    /// <summary>
    /// Interface for an object that can find <see cref="IContentReference"/>s
    /// for given search criteria.
    /// </summary>
    public interface IContentReferenceFinder
    {
        /// <summary>
        /// Finds all available content references.
        /// </summary>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The found content references.</returns>
        Task<IImmutableList<IContentReference>> FindAllAsync(CancellationToken cancel);

        /// <summary>
        /// Finds the content reference by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The found content reference, or null if no content reference was found.</returns>
        Task<IContentReference?> FindByIdAsync(Guid id, CancellationToken cancel);
    }

    /// <summary>
    /// Interface for an object that can find <see cref="IContentReference"/>s
    /// for a given content type and search criteria.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    public interface IContentReferenceFinder<TContent> : IContentReferenceFinder
        where TContent : IContentReference
    { }
}
