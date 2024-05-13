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
using System.Linq;
using System.Threading.Tasks;

namespace Tableau.Migration
{
    /// <summary>
    /// Class representing the disposable result of an operation.
    /// </summary>
    /// <typeparam name="T">The result's value type.</typeparam>
    internal sealed record AsyncDisposableResult<T> : Result<T>, IAsyncDisposableResult<T>
        where T : class, IAsyncDisposable
    {
        /// <summary>
        /// Creates a new <see cref="Result{T}"/> instance.
        /// </summary>
        /// <param name="success">True if the operation is successful, false otherwise.</param>
        /// <param name="value">The result of the operation.</param>
        /// <param name="errors">The errors encountered during the operation, if any.</param>
        private AsyncDisposableResult(bool success, T? value, params Exception[] errors)
            : base(success, value, errors)
        { }

        /// <summary>
        /// Creates a new <see cref="AsyncDisposableResult{T}"/> instance for successful operations.
        /// </summary>
        /// <param name="value">The result of the operation.</param>
        /// <returns>A new <see cref="AsyncDisposableResult{T}"/> instance.</returns>
        public static new AsyncDisposableResult<T> Succeeded(T value) => new(true, value);

        /// <summary>
        /// Creates a new <see cref="AsyncDisposableResult{T}"/> instance for failed operations.
        /// </summary>
        /// <param name="error">The error encountered during the operation.</param>
        /// <returns>A new <see cref="AsyncDisposableResult{T}"/> instance.</returns>
        public static new AsyncDisposableResult<T> Failed(Exception error) => Failed(new[] { error });

        /// <summary>
        /// Creates a new <see cref="AsyncDisposableResult{T}"/> instance for successful operations
        /// </summary>
        /// <param name="errors">The errors encountered during the operation.</param>
        /// <returns>A new <see cref="AsyncDisposableResult{T}"/> instance.</returns>
        public static new AsyncDisposableResult<T> Failed(IEnumerable<Exception> errors) => new(false, null, errors.ToArray());

        /// <summary>
        /// Creates a new <see cref="AsyncDisposableResult{T}"/> instance.
        /// </summary>
        /// <typeparam name="TResult">The type used to create the <typeparamref name="T"/> value.</typeparam>
        /// <param name="result">The result value used to create the <typeparamref name="T"/> value.</param>
        /// <param name="valueFactory">The method used to create the <typeparamref name="T"/> value.</param>
        /// <returns>A new <see cref="AsyncDisposableResult{T}"/> instance.</returns>
        public static new AsyncDisposableResult<T> Create<TResult>(TResult result, Func<TResult?, T> valueFactory) =>
            Create(result, valueFactory, Succeeded, Failed);

        /// <summary>
        /// Disposes the result's value.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            if (Value is not null)
                await Value.DisposeAsync().ConfigureAwait(false);

            GC.SuppressFinalize(this);
        }
    }
}
