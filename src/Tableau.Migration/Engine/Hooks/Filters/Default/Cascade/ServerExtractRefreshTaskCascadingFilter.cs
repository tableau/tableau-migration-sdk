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

using Microsoft.Extensions.Logging;
using Tableau.Migration.Content.Schedules.Server;
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Hooks.Filters.Default.Cascade
{
    /// <summary>
    /// Filter that excludes server extract refresh tasks based on cascading filters.
    /// </summary>
    public class ServerExtractRefreshTaskCascadingFilter : ExtractRefereshTaskCascadingFilterBase<IServerExtractRefreshTask, IServerSchedule>
    {
        /// <summary>
        /// Creates a new <see cref="ServerExtractRefreshTaskCascadingFilter"/> object.
        /// </summary>
        /// <param name="manifest"><inheritdoc /></param>
        /// <param name="localizer"><inheritdoc /></param>
        /// <param name="logger"><inheritdoc /></param>
        public ServerExtractRefreshTaskCascadingFilter(IMigrationManifestEditor manifest, 
            ISharedResourcesLocalizer? localizer, ILogger<ServerExtractRefreshTaskCascadingFilter>? logger) 
            : base(manifest, localizer, logger)
        { }
    }
}
