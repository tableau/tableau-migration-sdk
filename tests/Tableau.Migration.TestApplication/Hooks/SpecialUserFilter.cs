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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tableau.Migration.Content;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Hooks.Filters;
using Tableau.Migration.Resources;
using Tableau.Migration.TestApplication.Config;

namespace Tableau.Migration.TestApplication.Hooks
{
    public class SpecialUserFilter : ContentFilterBase<IUser>
    {
        private readonly TestApplicationOptions _options;

        public SpecialUserFilter(
            IOptions<TestApplicationOptions> options,
            ISharedResourcesLocalizer localizer, 
            ILogger<IContentFilter<IUser>> logger) : base(localizer, logger) 
        { 
            _options = options.Value;
        }


        public override bool ShouldMigrate(ContentMigrationItem<IUser> item)
        {
            // Do no migrate if it's in the specialEmail list
            return !_options.SpecialUsers.Emails.Contains(item.SourceItem.Email);
        }
    }
}
