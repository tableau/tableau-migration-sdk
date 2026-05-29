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
using Microsoft.Extensions.Logging;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Schedules;
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Hooks.Filters.Default.Cascade
{
    /// <summary>
    /// Abstract base class for a filter that can cascade filters to extract refresh tasks.
    /// </summary>
    /// <typeparam name="TTask">The extract refresh type.</typeparam>
    /// <typeparam name="TSchedule">The schedule type.</typeparam>
    public abstract class ExtractRefereshTaskCascadingFilterBase<TTask, TSchedule> : CascadingFilterBase<TTask>
        where TTask : IExtractRefreshTask<TSchedule>
        where TSchedule : ISchedule
    {
        /// <summary>
        /// Creates a new <see cref="ExtractRefereshTaskCascadingFilterBase{TTask, TSchedule}"/> object.
        /// </summary>
        /// <param name="manifest"><inheritdoc /></param>
        /// <param name="localizer"><inheritdoc /></param>
        /// <param name="logger"><inheritdoc /></param>
        protected ExtractRefereshTaskCascadingFilterBase(IMigrationManifestEditor manifest, 
            ISharedResourcesLocalizer? localizer, ILogger<ExtractRefereshTaskCascadingFilterBase<TTask, TSchedule>>? logger) 
            : base(manifest, localizer, logger)
        { }

        /// <inheritdoc />
        protected override bool HasExtraCascadingFilterReferences(ContentFilterContextItem<TTask> item)
        {
            switch(item.SourceItem.ContentType)
            {
                case ExtractRefreshContentType.DataSource:
                    return HasCascadingFilterReference<IDataSource>(item.SourceItem.Content);
                case ExtractRefreshContentType.Workbook:
                    return HasCascadingFilterReference<IWorkbook>(item.SourceItem.Content);
                case ExtractRefreshContentType.Unknown:
                    return false;
                default:
                    throw new ArgumentException($"Unsupported extract refresh content type: {item.SourceItem.ContentType}");
            }
        }
    }
}
