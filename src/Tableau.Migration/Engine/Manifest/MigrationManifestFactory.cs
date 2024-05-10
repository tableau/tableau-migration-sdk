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
using Microsoft.Extensions.Logging;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Manifest
{
    /// <summary>
    /// Default <see cref="IMigrationManifestFactory"/> implementation.
    /// </summary>
    public class MigrationManifestFactory : IMigrationManifestFactory
    {
        private readonly ISharedResourcesLocalizer _localizer;
        private readonly ILoggerFactory _loggerFactory;

        /// <summary>
        /// Creates a new <see cref="MigrationManifestFactory"/> object.
        /// </summary>
        /// <param name="localizer">A localizer.</param>
        /// <param name="loggerFactory">A logger factory.</param>
        public MigrationManifestFactory(ISharedResourcesLocalizer localizer, ILoggerFactory loggerFactory)
        {
            _localizer = localizer;
            _loggerFactory = loggerFactory;
        }

        /// <inheritdoc />
        public IMigrationManifestEditor Create(IMigrationInput input, Guid migrationId)
        {
            return new MigrationManifest(_localizer, _loggerFactory, input.Plan.PlanId, migrationId, input.PreviousManifest);
        }

        /// <inheritdoc />
        public IMigrationManifestEditor Create(Guid planId, Guid migrationId)
        {
            return new MigrationManifest(_localizer, _loggerFactory, planId, migrationId, null);
        }
    }
}
