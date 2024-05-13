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

namespace Tableau.Migration.Engine.Actions
{
    /// <summary>
    /// <see cref="IResult"/> object for a migration action.
    /// </summary>
    public interface IMigrationActionResult : IResult
    {
        /// <summary>
        /// Gets whether or not to perform the next action in the pipeline.
        /// </summary>
        bool PerformNextAction { get; }

        /// <summary>
        /// Creates a new <see cref="IMigrationActionResult"/> object while modifying the <see cref="PerformNextAction"/> value.
        /// </summary>
        /// <param name="performNextAction">Whether or not to perform the next action in the pipeline.</param>
        /// <returns>The new <see cref="IMigrationActionResult"/> object.</returns>
        IMigrationActionResult ForNextAction(bool performNextAction);
    }
}
