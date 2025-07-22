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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Engine.Pipelines;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Hooks.InitializeMigration.Capabilities
{
    /// <summary>
    /// Base class for <see cref="IMigrationCapabilityManager"/>s that are determined by source/destination endpoint settings.
    /// </summary>
    public abstract class EndpointSettingCapabilityManagerBase : MigrationCapabilityManagerBase
    {
        /// <summary>
        /// Creates a new <see cref="EndpointSettingCapabilityManagerBase"/> object.
        /// </summary>
        /// <param name="capabilities"><inheritdoc /></param>
        /// <param name="localizer"><inheritdoc /></param>
        /// <param name="logger"><inheritdoc /></param>
        protected EndpointSettingCapabilityManagerBase(IMigrationCapabilitiesEditor capabilities, 
            ISharedResourcesLocalizer localizer, ILogger logger)
            : base(capabilities, localizer, logger)
        { }

        /// <summary>
        /// Gets the content type(s) managed by this capability manager.
        /// </summary>
        protected abstract IEnumerable<Type> CapabilityContentTypes { get; }

        /// <summary>
        /// Gets the content type used for display text in log messages.
        /// </summary>
        protected virtual Type DisplayCapabilityContentTypes => CapabilityContentTypes.Single();

        /// <summary>
        /// Gets whether the endpoint has a setting disabling the content type(s) managed by this capability manager.
        /// </summary>
        /// <param name="ctx">The endpoint's preflight context.</param>
        /// <returns>True if the content type(s) are disabled, otherwise false.</returns>
        protected abstract bool GetEndpointDisabledSetting(IEndpointPreflightContext ctx);

        /// <inheritdoc />
        public override Task<IResult> SetMigrationCapabilityAsync(IInitializeMigrationContext ctx, CancellationToken cancel)
        {
            bool sourceDisabled = GetEndpointDisabledSetting(ctx.Source);
            bool destinationDisabled = GetEndpointDisabledSetting(ctx.Destination);

            if (sourceDisabled || destinationDisabled)
            {
                var endpoints = sourceDisabled && destinationDisabled ? "source and destination" : sourceDisabled ? "source" : "destination";

                Logger.LogInformation(Localizer[SharedResourceKeys.ContentTypeDisabledReason],
                    MigrationPipelineContentType.GetDisplayNameForType(DisplayCapabilityContentTypes, plural: true), endpoints);

                Capabilities.ContentTypesDisabledAtDestination.AddRange(CapabilityContentTypes);
            }

            return Task.FromResult<IResult>(Result.Succeeded());
        }
    }
}
