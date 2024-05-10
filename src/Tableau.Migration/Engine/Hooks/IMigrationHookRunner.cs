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

namespace Tableau.Migration.Engine.Hooks
{
    /// <summary>
    /// Interface for an object that can run hooks.
    /// </summary>
    public interface IMigrationHookRunner
    {
        /// <summary>
        /// Executes all hooks for the hook type in order.
        /// </summary>
        /// <typeparam name="THook">The hook type.</typeparam>
        /// <typeparam name="TContext">The hook context type.</typeparam>
        /// <param name="context">The context to pass to the first hook.</param>
        /// <param name="cancel"></param>
        /// <returns>The result context returned by the last hook.</returns>
        Task<TContext> ExecuteAsync<THook, TContext>(TContext context, CancellationToken cancel)
            where THook : IMigrationHook<TContext>;
    }
}
