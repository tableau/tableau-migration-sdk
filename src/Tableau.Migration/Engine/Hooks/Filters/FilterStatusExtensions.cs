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
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Hooks.Filters
{
    internal static class FilterStatusExtensions
    {
        internal static void ApplyAfterHook(this FilterStatus status, IMigrationManifestEntryEditor manifestEntry, 
            string hookName, ILogger logger, ISharedResourcesLocalizer localizer)
        {
            switch (status)
            {
                case FilterStatus.Migrate:
                    manifestEntry.ResetStatus();
                    break;
                case FilterStatus.Skip:
                case FilterStatus.CascadeSkip:
                    manifestEntry.SetSkipped(status is FilterStatus.CascadeSkip, hookName);

                    logger.LogDebug(localizer[SharedResourceKeys.ContentFilterBaseDebugMessage], hookName,
                            manifestEntry.Source.ToStringForLog());
                    break;
                default:
                    throw new NotSupportedException($"Content item filter status {status} is not supported.");
            }
        }
    }
}
