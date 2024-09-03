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

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Hooks.Transformers.Default
{
    /// <summary>
    /// Transformer that maps the users from a given group.
    /// </summary>
    public class GroupUsersTransformer : ContentTransformerBase<IPublishableGroup>
    {
        private readonly IDestinationContentReferenceFinder<IUser> _userFinder;

        /// <summary>
        /// Creates a new <see cref="GroupUsersTransformer"/> object.
        /// </summary>
        /// <param name="destinationFinderFactory">The destination finder factory.</param>
        /// <param name="localizer">A string localizer.</param>
        /// <param name="logger">The logger used to log messages.</param>
        public GroupUsersTransformer(
            IDestinationContentReferenceFinderFactory destinationFinderFactory,
            ISharedResourcesLocalizer localizer,
            ILogger<GroupUsersTransformer> logger) : base(localizer, logger)
        {
            _userFinder = destinationFinderFactory.ForDestinationContentType<IUser>();
        }

        /// <inheritdoc />
        public override async Task<IPublishableGroup?> TransformAsync(
            IPublishableGroup sourceGroup,
            CancellationToken cancel)
        {
            var missingUsers = new List<string>();

            foreach (var user in sourceGroup.Users)
            {
                var destinationUser = await _userFinder
                    .FindBySourceLocationAsync(user.User.Location, cancel)
                    .ConfigureAwait(false);

                if (destinationUser is null)
                {
                    missingUsers.Add(user.User.Name);
                    continue;
                }

                user.User = destinationUser;                
            }

            LogMissingUsers(sourceGroup.Name, missingUsers);
            return sourceGroup;
        }

        private void LogMissingUsers(string groupName, List<string> missingUsers)
        {
            if (!missingUsers.Any())
            {
                return;
            }

            Logger.LogDebug(
                       Localizer[SharedResourceKeys.GroupUsersTransformerCannotAddUserWarning],
                       groupName,
                       string.Join(',', missingUsers));
        }
    }
}
