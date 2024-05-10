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

using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Hooks.Mappings;
using Tableau.Migration.Engine.Hooks.Mappings.Default;
using Tableau.Migration.Engine.Options;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Resources;

namespace Tableau.Migration.TestApplication.Hooks
{
    /// <summary>
    /// Mapping that uses the base email plus a guid to create an email for every user.
    /// Example. sfroehlich@tableau.com would turn into sfroehlich+df334d533a96449e808e109d55e60cdd@tableau.com
    /// </summary>
    public class TestTableauCloudUsernameMapping : ContentMappingBase<IUser>, ITableauCloudUsernameMapping
    {
        private readonly MailAddress _baseEmail;
        private readonly bool _alwaysOverrideAddress;

        /// <summary>
        /// Creates a new <see cref="TestTableauCloudUsernameMapping"/> object.
        /// </summary>
        /// <param name="optionsProvider">The options for this Mapping.</param>
        /// <param name="localizer">A string localizer.</param>
        /// <param name="logger">Default logger.</param>
        public TestTableauCloudUsernameMapping(
            IMigrationPlanOptionsProvider<TestTableauCloudUsernameOptions> optionsProvider,
            ISharedResourcesLocalizer localizer,
            ILogger<TestTableauCloudUsernameMapping> logger) 
                : base(localizer, logger)
        {
            _baseEmail = new MailAddress(optionsProvider.Get().BaseOverrideMailAddress);
            _alwaysOverrideAddress = optionsProvider.Get().AlwaysOverrideAddress;
        }

        /// <summary>
        /// Adds an email to the user if it doesn't exist. 
        /// </summary>
        public override Task<ContentMappingContext<IUser>?> MapAsync(ContentMappingContext<IUser> ctx, CancellationToken cancel)
        {
            var domain = ctx.MappedLocation.Parent();
            // Re-use an existing email if it already exists unless always override is set
            if (_alwaysOverrideAddress == false && !string.IsNullOrEmpty(ctx.ContentItem.Email))
                return ctx.MapTo(domain.Append(ctx.ContentItem.Email)).ToTask();

            // Takes the existing "Name" and appends the default domain to build the email
            var testEmail = $"{_baseEmail.User}+{ctx.ContentItem.Name.Replace(" ", "")}@{_baseEmail.Host}";
            return ctx.MapTo(domain.Append(testEmail)).ToTask();
        }
    }
}