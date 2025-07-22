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
using System.Linq;

namespace Tableau.Migration.Engine.Hooks.InitializeMigration
{
    internal record InitializeMigrationHookResult : Result, IInitializeMigrationHookResult
    {
        /// <inheritdoc />
        public IServiceProvider ScopedServices { get; }

        /// <inheritdoc />
        public IEndpointPreflightContext Source { get; }

        /// <inheritdoc />
        public IEndpointPreflightContext Destination { get; }

        protected InitializeMigrationHookResult(bool success, IServiceProvider scopedServices, 
            IEndpointPreflightContext source, IEndpointPreflightContext destination,
            params IEnumerable<Exception> errors)
            : base(success, errors)
        {
            ScopedServices = scopedServices;
            Source = source;
            Destination = destination;
        }

        public static InitializeMigrationHookResult Succeeded(IServiceProvider scopedServices, IEndpointPreflightContext source, IEndpointPreflightContext destination) 
            => new(true, scopedServices, source, destination);

        public IInitializeMigrationHookResult ToFailure(params IEnumerable<Exception> errors)
            => new InitializeMigrationHookResult(false, ScopedServices, Source, Destination, Errors.Concat(errors));
    }
}