﻿//
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

namespace Tableau.Migration.Engine.Hooks.InitializeMigration
{
    /// <summary>
    /// <see cref="IResult"/> object for an <see cref="IInitializeMigrationHook"/>.
    /// </summary>
    public interface IInitializeMigrationHookResult : IInitializeMigrationContext, IResult
    {
        /// <summary>
        /// Creates a new <see cref="IInitializeMigrationHookResult"/> object with the given errors.
        /// </summary>
        /// <param name="errors">The errors that caused the failure.</param>
        /// <returns>The new <see cref="IInitializeMigrationHookResult"/> object.</returns>
        IInitializeMigrationHookResult ToFailure(params IEnumerable<Exception> errors);
    }
}