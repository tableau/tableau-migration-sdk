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
using System.Collections.Immutable;

namespace Tableau.Migration
{
    /// <summary>
    /// Object that can build <see cref="IResult"/> objects from aggregate results.
    /// </summary>
    internal class ResultBuilder
    {
        private readonly ImmutableArray<Exception>.Builder _errors;

        /// <summary>
        /// Creates a new <see cref="ResultBuilder"/> object.
        /// </summary>
        public ResultBuilder()
        {
            _errors = ImmutableArray.CreateBuilder<Exception>();
        }

        /// <summary>
        /// Adds the errors of the result to the aggregated result.
        /// </summary>
        /// <param name="results">The results to add errors from.</param>
        /// <returns>This result builder for fluent API usage.</returns>
        public virtual ResultBuilder Add(params IResult[] results)
        {
            foreach (var result in results)
            {
                _errors.AddRange(result.Errors);
            }

            return this;
        }

        /// <summary>
        /// Adds the error to the aggregated result.
        /// </summary>
        /// <param name="error">The error to add.</param>
        /// <returns>This result builder for fluent API usage.</returns>
        public virtual void Add(Exception error) => _errors.Add(error);

        /// <summary>
        /// Adds the errors to the aggregated result.
        /// </summary>
        /// <param name="errors">The errors to add.</param>
        /// <returns>This result builder for fluent API usage.</returns>
        public virtual void Add(IEnumerable<Exception> errors) => _errors.AddRange(errors);

        /// <summary>
        /// Builds the end result.
        /// </summary>
        /// <returns>The built result.</returns>
        public virtual IResult Build() => Result.FromErrors(_errors);

        /// <summary>
        /// Builds the end result.
        /// </summary>
        /// <param name="value">The value to use if the result is a success.</param>
        /// <returns>The built result.</returns>
        public virtual IResult<T> Build<T>(T? value)
            where T : class
            => Result<T>.Create(Build(), value);
    }
}
