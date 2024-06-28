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

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Hooks.Filters;

namespace Tableau.Migration.TestApplication.Hooks
{
    class SkipFilter<TContent> : IContentFilter<TContent>
        where TContent : IContentReference
    {
        public Task<IEnumerable<ContentMigrationItem<TContent>>?> ExecuteAsync(IEnumerable<ContentMigrationItem<TContent>> ctx, CancellationToken token)
        {
            return Task.FromResult<IEnumerable<ContentMigrationItem<TContent>>?>(
                Enumerable.Empty<ContentMigrationItem<TContent>>());
        }
    }
}
