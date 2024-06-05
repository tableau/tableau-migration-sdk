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

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Hooks.Mappings;
using Tableau.Migration.Resources;
using Tableau.Migration.TestApplication.Config;

namespace Tableau.Migration.TestApplication.Hooks
{
    public class SpecialUserMapping : ContentMappingBase<IUser>
    {
        private readonly TestApplicationOptions _options;
        private readonly ContentLocation _adminUser;

        public SpecialUserMapping(
            IOptions<TestApplicationOptions> options,
            ISharedResourcesLocalizer localizer,
            ILogger<IContentMapping<IUser>> logger)
                : base(localizer, logger)
        {
            _options = options.Value;

            if (string.IsNullOrEmpty(_options.SpecialUsers.AdminUsername) || string.IsNullOrEmpty(_options.SpecialUsers.AdminDomain))
                throw new Exception("SpecialUserMapping requires an Admin domain and username");

            _adminUser = ContentLocation.ForUsername(_options.SpecialUsers.AdminDomain, _options.SpecialUsers.AdminUsername);
        }


        public override Task<ContentMappingContext<IUser>?> MapAsync(ContentMappingContext<IUser> ctx, CancellationToken cancel)
        {
            if(_options.SpecialUsers.Emails.Contains(ctx.ContentItem.Email))
            {
                ctx = ctx.MapTo(_adminUser);
            }

            return ctx.ToTask();
        }
    }
}
