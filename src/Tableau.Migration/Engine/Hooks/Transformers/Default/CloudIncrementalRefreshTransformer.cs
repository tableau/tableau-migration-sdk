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

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Content.Schedules.Cloud;


using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Hooks.Transformers.Default
{
    /// <summary>
    /// Transformer that changes extract refresh tasks to cloud supported ones.     
    /// </summary>       
    public class CloudIncrementalRefreshTransformer(
        ISharedResourcesLocalizer localizer,
        ILogger<CloudIncrementalRefreshTransformer> logger)
        : ContentTransformerBase<ICloudExtractRefreshTask>(localizer, logger)
    {

        /// <inheritdoc />
        public override Task<ICloudExtractRefreshTask?> TransformAsync(
            ICloudExtractRefreshTask itemToTransform,
            CancellationToken cancel)
        {
            // Convert Server Incremental Refresh to Cloud Incremental Refresh
            if (itemToTransform.Type == ExtractRefreshType.ServerIncrementalRefresh)
            {
                itemToTransform.Type = ExtractRefreshType.CloudIncrementalRefresh;
            }

            return Task.FromResult<ICloudExtractRefreshTask?>(itemToTransform);
        }
    }
}
