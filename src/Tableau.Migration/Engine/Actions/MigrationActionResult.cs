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

namespace Tableau.Migration.Engine.Actions
{
    /// <summary>
    /// Default <see cref="IMigrationActionResult"/> implementation.
    /// </summary>
    internal record MigrationActionResult : Result, IMigrationActionResult
    {
        /// <inheritdoc />
        public bool PerformNextAction { get; }

        protected MigrationActionResult(bool success, bool performNextAction, IEnumerable<Exception> errors)
            : base(success, errors)
        {
            PerformNextAction = performNextAction;
        }

        protected MigrationActionResult(IResult baseResult, bool performNextAction)
            : this(baseResult.Success, performNextAction, baseResult.Errors)
        { }

        protected MigrationActionResult(bool success, bool performNextAction, params Exception[] errors)
            : this(success, performNextAction, (IEnumerable<Exception>)errors)
        { }

        /// <summary>
        /// Creates a new <see cref="MigrationActionResult"/> instance for successful operations.
        /// </summary>
        /// <param name="performNextAction">Whether or not to perform the next action in the pipeline.</param>
        /// <returns>A new <see cref="MigrationActionResult"/> instance.</returns>
        public static MigrationActionResult Succeeded(bool performNextAction = true)
            => new(true, performNextAction);

        /// <summary>
        /// Creates a new <see cref="MigrationActionResult"/> instance for failed operations.
        /// </summary>
        /// <param name="error">The error encountered during the operation.</param>
        /// <param name="performNextAction">Whether or not to perform the next action in the pipeline.</param>
        /// <returns>A new <see cref="MigrationActionResult"/> instance.</returns>
        public static MigrationActionResult Failed(Exception error, bool performNextAction = true) => Failed(new[] { error }, performNextAction);

        /// <summary>
        /// Creates a new <see cref="MigrationActionResult"/> instance for successful operations.
        /// </summary>
        /// <param name="errors">The errors encountered during the operation.</param>
        /// <param name="performNextAction">Whether or not to perform the next action in the pipeline.</param>
        /// <returns>A new <see cref="MigrationActionResult"/> instance.</returns>
        public static MigrationActionResult Failed(IEnumerable<Exception> errors, bool performNextAction = true)
            => new(false, performNextAction, errors);

        /// <summary>
        /// Creates a new <see cref="MigrationActionResult"/> instance based on the given result.
        /// </summary>
        /// <param name="result">The inner result to get success/errors from.</param>
        /// <param name="performNextActionOverride">Whether or not to perform the next action, or null to base this on the success of <paramref name="result" />.</param>
        /// <returns>A new <see cref="MigrationActionResult"/> instance.</returns>
        public static MigrationActionResult FromResult(IResult result, bool? performNextActionOverride = null)
            => new(result, performNextActionOverride ?? result.Success);

        /// <inheritdoc />
        public IMigrationActionResult ForNextAction(bool performNextAction)
            => new MigrationActionResult(this, performNextAction);
    }
}
