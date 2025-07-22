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

using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Endpoints;

namespace Tableau.Migration.Engine.Hooks.PostPublish.Default
{
    internal sealed class PopulateViewCachePostPublishHook : ContentItemPostPublishHookBase<IPublishableWorkbook, IWorkbookDetails>
    {
        private readonly IMigration _migration;

        public PopulateViewCachePostPublishHook(IMigration migration)
        {
            _migration = migration;
        }

        private static void PopulateViewCaches(IMigrationEndpoint endpoint, IWorkbookDetails workbook)
        {
            var viewCache = endpoint.GetViewCache();
            foreach(var view in workbook.Views)
            {
                viewCache.Add(view.Id, view);
            }

            var workbookViewsCache = endpoint.GetWorkbookViewsCache();
            workbookViewsCache.Add(workbook.Id, workbook.Views);
        }

        public override Task<ContentItemPostPublishContext<IPublishableWorkbook, IWorkbookDetails>?> ExecuteAsync(
            ContentItemPostPublishContext<IPublishableWorkbook, IWorkbookDetails> ctx, CancellationToken cancel)
        {
            PopulateViewCaches(_migration.Source, ctx.PublishedItem);
            PopulateViewCaches(_migration.Destination, ctx.DestinationItem);

            return Task.FromResult<ContentItemPostPublishContext<IPublishableWorkbook, IWorkbookDetails>?>(ctx);
        }
    }
}
