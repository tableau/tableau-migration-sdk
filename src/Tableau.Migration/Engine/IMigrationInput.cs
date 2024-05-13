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

namespace Tableau.Migration.Engine
{
    /// <summary>
    /// Interface for an object that contains the input given for a <see cref="IMigration"/>, 
    /// used to bootstrap migration engine dependency injection.
    /// </summary>
    /// <remarks>
    /// In almost all cases it is preferrable to inject the <see cref="IMigration"/> object, 
    /// this interface is only intended to be used to build <see cref="IMigration"/> object.
    /// </remarks>
    public interface IMigrationInput
    {
        /// <summary>
        /// Gets the unique identifier of the migration.
        /// This is generated as part of the input so that migration bootstrapping can avoid DI cycles.
        /// </summary>
        Guid MigrationId { get; }

        /// <summary>
        /// Gets the migration plan to execute.
        /// </summary>
        IMigrationPlan Plan { get; }

        /// <summary>
        /// Gets a manifest from a previous migration of the same plan to use to determine what progress has already been made.
        /// </summary>
        IMigrationManifest? PreviousManifest { get; }
    }
}
