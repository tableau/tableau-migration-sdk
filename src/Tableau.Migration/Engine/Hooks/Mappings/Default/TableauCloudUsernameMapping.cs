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
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Options;

namespace Tableau.Migration.Engine.Hooks.Mappings.Default
{
    /// <summary>
    /// Default <see cref="ITableauCloudUsernameMapping"/> implementation.
    /// </summary>
    public class TableauCloudUsernameMapping : ITableauCloudUsernameMapping
    {
        private readonly string _mailDomain;
        private readonly bool _useExistingEmail;

        /// <summary>
        /// Creates a new <see cref="TableauCloudUsernameMapping"/> object.
        /// </summary>
        /// <param name="optionsProvider">The options provider.</param>
        public TableauCloudUsernameMapping(IMigrationPlanOptionsProvider<TableauCloudUsernameMappingOptions> optionsProvider)
        {
            var opts = optionsProvider.Get();

            _mailDomain = opts.MailDomain;
            _useExistingEmail = opts.UseExistingEmail;
        }

        /// <inheritdoc />
        public Task<ContentMappingContext<IUser>?> ExecuteAsync(ContentMappingContext<IUser> ctx, CancellationToken cancel)
        {
            if (_useExistingEmail && !string.IsNullOrWhiteSpace(ctx.ContentItem.Email))
            {
                var emailLocation = ctx.MappedLocation.Parent().Append(ctx.ContentItem.Email);
                return ctx.MapTo(emailLocation).ToTask();
            }

            var generatedUsername = $"{ctx.MappedLocation.Name}@{_mailDomain}";
            var generatedLocation = ctx.MappedLocation.Parent().Append(generatedUsername);
            return ctx.MapTo(generatedLocation).ToTask();
        }
    }
}
