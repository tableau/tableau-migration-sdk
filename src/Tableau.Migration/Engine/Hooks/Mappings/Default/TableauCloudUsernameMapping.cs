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

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Options;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Hooks.Mappings.Default
{
    /// <summary>
    /// Default <see cref="ITableauCloudUsernameMapping"/> implementation.
    /// </summary>
    public class TableauCloudUsernameMapping : ITableauCloudUsernameMapping
    {
        private readonly string _mailDomain;
        private readonly bool _useExistingEmail;
        private readonly ISharedResourcesLocalizer _localizer;
        private readonly ILogger<TableauCloudUsernameMapping>? _logger;

        /// <summary>
        /// Creates a new <see cref="TableauCloudUsernameMapping"/> object.
        /// </summary>
        /// <param name="optionsProvider">The options provider.</param>
        /// <param name="localizer">A string localizer.</param>
        /// <param name="logger">Default logger.</param>
        public TableauCloudUsernameMapping(
            IMigrationPlanOptionsProvider<TableauCloudUsernameMappingOptions> optionsProvider,
            ISharedResourcesLocalizer localizer,
            ILogger<TableauCloudUsernameMapping>? logger)
        {
            var opts = optionsProvider.Get();

            _mailDomain = opts.MailDomain;
            _useExistingEmail = opts.UseExistingEmail;

            _localizer = localizer;
            _logger = logger;
        }

        /// <inheritdoc />
        public Task<ContentMappingContext<IUser>?> ExecuteAsync(ContentMappingContext<IUser> ctx, CancellationToken cancel)
        {
            ContentMappingContext<IUser> ret;

            if (_useExistingEmail && !string.IsNullOrWhiteSpace(ctx.ContentItem.Email))
            {
                var emailLocation = ctx.MappedLocation.Parent().Append(ctx.ContentItem.Email);
                ret = ctx.MapTo(emailLocation);
            }
            else
            {
                var generatedUsername = $"{ctx.MappedLocation.Name}@{_mailDomain}";
                var generatedLocation = ctx.MappedLocation.Parent().Append(generatedUsername);
                ret = ctx.MapTo(generatedLocation);
            }

            if (_logger is not null && _localizer is not null && ret is not null)
            {
                if (ctx.MappedLocation != ret.MappedLocation)
                {
                    _logger.LogDebug(
                        _localizer[SharedResourceKeys.ContentMappingBaseDebugMessage],
                        GetType().Name,
                        ret.ContentItem.ToStringForLog(),
                        ret.MappedLocation);
                }
            }

            return ret?.ToTask() ?? Task.FromResult<ContentMappingContext<IUser>?>(null);
        }
    }
}
