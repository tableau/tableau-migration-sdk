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
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Content;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Hooks.Filters;
using Tableau.Migration.Resources;
using Tableau.Migration.TestApplication.Config;

namespace Tableau.Migration.TestApplication.Hooks
{
    public class NonDomainUserFilter : ContentFilterBase<IUser>
    {
        private readonly HashSet<string> _samAccountNames = new HashSet<string>();
        private readonly TestApplicationOptions _options;

        public NonDomainUserFilter(
            IOptions<TestApplicationOptions> options, 
            ISharedResourcesLocalizer localizer,
            ILogger<IContentFilter<IUser>> logger) : base (localizer, logger)
        {
            _options = options.Value;

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                throw new NotSupportedException("Must be run on Windows");

            // The following code enumerates all users in the domain, and saves their usernames for lookup if they're still available. 

            // Create a DirectoryEntry object with the domain name and credentials
            var entry = new DirectoryEntry(_options.Domain.Path, _options.Domain.Username, _options.Domain.Password);

            // Create a DirectorySearcher object with the entry and the filter
            DirectorySearcher search = new DirectorySearcher(entry);
            search.Filter = "(objectClass=user)";

            // Load the account name, which is the username
            search.PropertiesToLoad.Add("samaccountname");

            // Get all the users
            SearchResultCollection results = search.FindAll();

            foreach (SearchResult sr in results)
            {
                var samaccountname = sr.Properties["samaccountname"][0].ToString();

                if (!string.IsNullOrEmpty(samaccountname))
                    _samAccountNames.Add(samaccountname);
            }
        }

        public override bool ShouldMigrate(ContentMigrationItem<IUser> item)
            => item.SourceItem.LicenseLevel != LicenseLevels.Unlicensed && _samAccountNames.Contains(item.SourceItem.Name, StringComparer.OrdinalIgnoreCase);
    }
}
