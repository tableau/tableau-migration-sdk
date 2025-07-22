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
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tableau.Migration.Content;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Hooks.Filters;
using Tableau.Migration.Resources;
using Tableau.Migration.TestApplication.Config;

namespace Tableau.Migration.TestApplication.Hooks.Filters
{
    public class SkipIdsFilter<TContent> : ContentFilterBase<TContent>
        where TContent : IContentReference
    {
        private readonly Guid[] idsToSkip;

        public SkipIdsFilter(
            IOptions<TestApplicationOptions> options,
            ISharedResourcesLocalizer localizer,
            ILogger<SkipIdsFilter<TContent>> logger) : base(localizer, logger)
        {
            Type filterType = typeof(TContent);

            if (filterType == typeof(IUser))
            {
                idsToSkip = options.Value.SkipIds.UserGuids;
            }
            else if (filterType == typeof(IGroup))
            {
                idsToSkip = options.Value.SkipIds.GroupGuids;
            }
            else if (filterType == typeof(IProject))
            {
                idsToSkip = options.Value.SkipIds.ProjectGuids;
            }
            else if (filterType == typeof(IDataSource))
            {
                idsToSkip = options.Value.SkipIds.DatasourceGuids;
            }
            else if (filterType == typeof(IWorkbook))
            {
                idsToSkip = options.Value.SkipIds.WorkbookGuids;
            }
            else
            {
                idsToSkip = Array.Empty<Guid>();
            }
        }


        public override bool ShouldMigrate(ContentMigrationItem<TContent> item)
        {
            if (idsToSkip.Contains(item.SourceItem.Id))
            {
                return false;
            }

            return true;
        }
    }
}
