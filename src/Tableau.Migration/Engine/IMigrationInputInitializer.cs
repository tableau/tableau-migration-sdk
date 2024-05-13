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

namespace Tableau.Migration.Engine
{
    /// <summary>
    /// Interface for an object that can initialize a <see cref="IMigrationInput"/> object.
    /// </summary>
    /// <remarks>
    /// This interface is internal because it is only used to build a <see cref="IMigrationInput"/> object, 
    /// which in turn is only used to build a <see cref="IMigration"/> object.
    /// End users are intended to inject the final <see cref="IMigration"/> result and not bootstrap objects.
    /// </remarks>
    internal interface IMigrationInputInitializer
    {
        /// <summary>
        /// Initializes the <see cref="IMigrationInput"/> object.
        /// </summary>
        /// <param name="plan">The migration plan to execute.</param>
        /// <param name="previousManifest">A manifest from a previous migration of the same plan to use to determine what progress has already been made.</param>
        void Initialize(IMigrationPlan plan, IMigrationManifest? previousManifest);
    }
}
