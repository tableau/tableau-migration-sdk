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
using Microsoft.Extensions.Logging;
using Tableau.Migration.Content;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Hooks.InitializeMigration.Capabilities
{
    internal sealed class SubscriptionsCapabilityManager : EndpointSettingCapabilityManagerBase
    {
        public SubscriptionsCapabilityManager(IMigrationCapabilitiesEditor capabilities,
            ISharedResourcesLocalizer localizer, ILogger<SubscriptionsCapabilityManager> logger)
            : base(capabilities, localizer, logger)
        { }

        /// <inheritdoc />
        protected override IEnumerable<Type> CapabilityContentTypes => [typeof(IServerSubscription), typeof(ICloudSubscription)];

        /// <inheritdoc />
        protected override Type DisplayCapabilityContentTypes => typeof(ISubscription<>);

        /// <inheritdoc />
        protected override bool GetEndpointDisabledSetting(IEndpointPreflightContext ctx)
            => ctx.Session.Settings?.DisableSubscriptions == true;
    }
}
