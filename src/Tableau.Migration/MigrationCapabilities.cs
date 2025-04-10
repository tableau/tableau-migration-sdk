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
    /// Represents the capabilities of a migration and are set via <see cref="Engine.Hooks.IInitializeMigrationHook"/>s.
    /// </summary>
    internal class MigrationCapabilities : IMigrationCapabilitiesEditor
    {
        /// <inheritdoc/>
        public bool PreflightCheckExecuted { get; set; } = false;

        /// <inheritdoc/>
        public bool EmbeddedCredentialsDisabled { get; set; } = false;

        /// <inheritdoc/>
        public HashSet<Type> ContentTypesDisabledAtDestination { get; set; } = [];

        /// <inheritdoc/>
        public IMigrationCapabilities Clone()
        {
            return new MigrationCapabilities
            {
                PreflightCheckExecuted = PreflightCheckExecuted,
                EmbeddedCredentialsDisabled = EmbeddedCredentialsDisabled,
                ContentTypesDisabledAtDestination = ContentTypesDisabledAtDestination
            };
        }

        /// <inheritdoc/>
        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}
