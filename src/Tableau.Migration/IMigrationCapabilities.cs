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

using System;
using System.Collections.Generic;

namespace Tableau.Migration
{
    /// <summary>
    /// Represents the readonly capabilities of a migration.
    /// </summary>
    public interface IMigrationCapabilities : ICloneable
    {
        /// <summary>
        /// Gets a value indicating whether the preflight check has been executed.
        /// </summary>
        bool PreflightCheckExecuted { get; }

        /// <summary>
        /// Gets the unique list of items that are disabled at the destination.
        /// </summary>
        HashSet<Type> ContentTypesDisabledAtDestination { get; }

        /// <summary>
        /// Gets whether Embedded Credential migration is disabled.
        /// </summary>
        bool EmbeddedCredentialsDisabled { get; }

        /// <summary>
        /// Creates a new instance of <see cref="IMigrationCapabilities"/> that is a copy of the current instance.
        /// </summary>
        /// <returns>A new <see cref="IMigrationCapabilities"/> object that is a copy of this instance.</returns>
        new IMigrationCapabilities Clone();
    }
}
