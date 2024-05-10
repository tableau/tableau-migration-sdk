﻿//
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

using Tableau.Migration.Engine.Migrators.Batch;

namespace Tableau.Migration.Engine.Hooks
{
    /// <summary>
    /// Interface representing a hook called when a <see cref="IContentBatchMigrator{TContent}"/> completes.
    /// </summary>
    public interface IContentBatchMigrationCompletedHook<TContent> : IMigrationHook<IContentBatchMigrationResult<TContent>>
        where TContent : IContentReference
    { }
}
