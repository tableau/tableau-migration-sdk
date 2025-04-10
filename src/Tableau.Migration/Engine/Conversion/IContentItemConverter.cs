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

using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Engine.Conversion
{
    /// <summary>
    /// Interface for an object that can convert items during migration.
    /// </summary>
    /// <typeparam name="TPrepare">The type of item being prepared from the source.</typeparam>
    /// <typeparam name="TPublish">The type of item to be published to the destination.</typeparam>
    public interface IContentItemConverter<TPrepare, TPublish>
    {
        /// <summary>
        /// Converts the item to a publishable type.
        /// </summary>
        /// <param name="sourceItem">The item being prepared for publishing.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The converted item.</returns>
        Task<TPublish> ConvertAsync(TPrepare sourceItem, CancellationToken cancel);
    }
}
