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

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Content;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Hooks.Filters;
using Tableau.Migration.Engine.Hooks.Mappings;
using Tableau.Migration.Resources;
using Tableau.Migration.TestApplication.Config;

namespace Tableau.Migration.TestApplication.Hooks
{
    public class UnlicensedUserMapping : ContentMappingBase<IUser>
    {
        private readonly ContentLocation _adminUser;

        public UnlicensedUserMapping(
            IOptions<TestApplicationOptions> options,
            ISharedResourcesLocalizer localizer,
            ILogger<UnlicensedUserMapping> logger) : base(localizer, logger)
        {
            _adminUser = ContentLocation.ForUsername(options.Value.SpecialUsers.AdminDomain, options.Value.SpecialUsers.AdminUsername);
        }

        public override Task<ContentMappingContext<IUser>?> MapAsync(ContentMappingContext<IUser> ctx, CancellationToken cancel)
        {
            if (ctx.ContentItem.LicenseLevel == LicenseLevels.Unlicensed)
            {
                ctx = ctx.MapTo(_adminUser);
            }

            return ctx.ToTask();
        }
    }
}
