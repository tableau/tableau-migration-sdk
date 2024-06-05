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

namespace Tableau.Migration
{
    /// <summary>
    /// Static class containing extension methods for <see cref="Task"/> and <see cref="Task{TResult}"/> objects.
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// Get the results synchronously, applying best practices.
        /// </summary>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="task">The task to get synchronous results from.</param>
        /// <returns>The task's result.</returns>
        public static TResult AwaitResult<TResult>(this Task<TResult> task) => task.ConfigureAwait(false).GetAwaiter().GetResult();
    }
}
