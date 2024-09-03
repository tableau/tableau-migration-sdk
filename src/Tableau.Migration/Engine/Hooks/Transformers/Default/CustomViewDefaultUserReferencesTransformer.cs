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
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Hooks.Transformers.Default
{
    /// <summary>
    /// Transformer that transforms the list of users that have the custom view as default.
    /// It sets the references of these users to those at the destination.
    /// </summary>
    public class CustomViewDefaultUserReferencesTransformer
        : ContentTransformerBase<IPublishableCustomView>
    {
        private readonly IMappedUserTransformer _userTransformer;

        /// <summary>
        /// Creates a new <see cref="CustomViewDefaultUserReferencesTransformer"/> object.
        /// </summary>
        /// <param name="userTransformer">The user transformer.</param>
        /// <param name="logger">The logger used to log messages.</param>
        /// <param name="localizer">The string localizer.</param>
        public CustomViewDefaultUserReferencesTransformer(
            IMappedUserTransformer userTransformer,
            ILogger<CustomViewDefaultUserReferencesTransformer> logger,
            ISharedResourcesLocalizer localizer)
            : base(localizer, logger)
        {
            _userTransformer = userTransformer;
        }

        /// <inheritdoc />
        public override async Task<IPublishableCustomView?> TransformAsync(
            IPublishableCustomView sourceCustomView,
            CancellationToken cancel)
        {
            var missingUsers = new List<string>();

            for (var i = 0; i < sourceCustomView.DefaultUsers.Count; i++)
            {
                var destinationUser = await _userTransformer.ExecuteAsync(sourceCustomView.DefaultUsers[i], cancel)
                    .ConfigureAwait(false);

                if (destinationUser is null)
                {
                    missingUsers.Add(sourceCustomView.DefaultUsers[i].Name);
                    continue;
                }
                sourceCustomView.DefaultUsers[i] = destinationUser;
            }

            LogMissingUsers(sourceCustomView.Name, missingUsers);

            return sourceCustomView;
        }

        private void LogMissingUsers(string customViewName, List<string> missingUsers)
        {
            if (!missingUsers.Any())
            {
                return;
            }

            Logger.LogDebug(
                Localizer[SharedResourceKeys.CustomViewDefaultUsersTransformerNoUserRefsDebugMessage],
                customViewName,
                string.Join(',', missingUsers));
        }
    }
}
