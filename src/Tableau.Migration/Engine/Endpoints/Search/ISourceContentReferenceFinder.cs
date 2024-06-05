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
using System.Threading;
using Tableau.Migration.Content.Search;

namespace Tableau.Migration.Engine.Endpoints.Search
{
    /// <summary>
    /// Interface for an object that can find source content reference.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    public interface ISourceContentReferenceFinder<TContent> : IContentReferenceFinder<TContent>
        where TContent : IContentReference
    {
        /// <summary>
        /// Finds the source content reference for the source content reference location.
        /// </summary>
        /// <param name="sourceLocation">The source content reference location.</param>
        /// <param name="cancel">A cancellation token to obey.</param>
        /// <returns>The found source content reference, or null if no content reference was found.</returns>
        Task<IContentReference?> FindBySourceLocationAsync(ContentLocation sourceLocation, CancellationToken cancel);
    }
}
