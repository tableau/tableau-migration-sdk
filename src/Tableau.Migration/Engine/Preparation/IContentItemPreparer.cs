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

using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Engine.Preparation
{
    /// <summary>
    /// Interface for an object that can prepare a content item for publishing.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    /// <typeparam name="TPublish">The publish type.</typeparam>
    public interface IContentItemPreparer<TContent, TPublish>
        where TPublish : class
    {
        /// <summary>
        /// Prepares a content item for publishing.
        /// </summary>
        /// <param name="item">The item to prepare.</param>
        /// <param name="cancel">A cancellation token to obye.</param>
        /// <returns>The preparation result.</returns>
        Task<IResult<TPublish>> PrepareAsync(ContentMigrationItem<TContent> item, CancellationToken cancel);
    }
}
