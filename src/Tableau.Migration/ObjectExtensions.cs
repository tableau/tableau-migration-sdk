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
using System.Threading.Tasks;

namespace Tableau.Migration
{
    internal static class ObjectExtensions
    {
        /// <summary>
        /// Disposes of a resource if the function throws, before re-throwing the exception.
        /// This ensures resources are disposed if they can't be returned. 
        /// </summary>
        /// <typeparam name="TReturn">The return type.</typeparam>
        /// <param name="resource">The resource to dispose if the function throws.</param>
        /// <param name="asyncFunction">The function that may throw.</param>
        /// <returns>The result of the function.</returns>
        public static async Task<TReturn> DisposeOnThrowAsync<TReturn>(this object? resource, Func<Task<TReturn>> asyncFunction)
        {
            try
            {
                var result = await asyncFunction().ConfigureAwait(false);

                return result;
            }
            catch
            {
                /* If we throw (even from cancellation) before we can return the resource,
                 * the resource can't be disposed by calling code.
                 * Orphaned resources are cleaned up at the end of the DI scope,
                 * but with paging/batching this means the resources may needlessly bloat
                 * resource/disk usage in a way that accumulates over time.*/

                await resource.DisposeIfNeededAsync().ConfigureAwait(false);
                throw;
            }
        }

        /// <summary>
        /// Disposes of a resource if the function throws or returns a failure result.
        /// This ensures resources are disposed if they can't be returned. 
        /// </summary>
        /// <typeparam name="TReturn">The return result type.</typeparam>
        /// <param name="resource">The resource to dispose if the function throws or returns a failure.</param>
        /// <param name="asyncFunction">The function that may throw or fail.</param>
        /// <returns>The result of the function.</returns>
        public static async Task<TReturn> DisposeOnThrowOrFailureAsync<TReturn>(this object? resource, Func<Task<TReturn>> asyncFunction)
            where TReturn : IResult
        {
            var result = await resource.DisposeOnThrowAsync(asyncFunction).ConfigureAwait(false);
            if(!result.Success)
            {
                await resource.DisposeIfNeededAsync().ConfigureAwait(false);
            }

            return result;
        }

        /// <summary>
        /// Disposes the object if it implements any disposable interfaces, 
        /// preferring asynchronous disposal.
        /// </summary>
        /// <param name="o">The object to try to dispose.</param>
        /// <returns>A task to await.</returns>
        public static async Task DisposeIfNeededAsync(this object? o)
        {
            if (o is null)
            {
                return;
            }

            if (o is IAsyncDisposable asyncDisposable)
            {
                await asyncDisposable.DisposeAsync().ConfigureAwait(false);
            }
            else if (o is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
