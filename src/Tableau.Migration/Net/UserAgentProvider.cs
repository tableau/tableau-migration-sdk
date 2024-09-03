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

using System.Text;
using Tableau.Migration.Config;

namespace Tableau.Migration.Net
{
    /// <summary>
    /// Default <see cref="IUserAgentProvider"/> implementation.
    /// </summary>
    internal sealed class UserAgentProvider : IUserAgentProvider
    {
        public UserAgentProvider(IMigrationSdk sdk, IConfigReader config)
        {
            var userAgentBuilder = new StringBuilder(Constants.USER_AGENT_PREFIX);
            userAgentBuilder.Append("/");
            userAgentBuilder.Append(sdk.Version);

            var comments = config.Get().Network.UserAgentComment;
            if (!string.IsNullOrWhiteSpace(comments))
            {
                userAgentBuilder.AppendFormat(" ({0})", comments);
            }

            UserAgent = userAgentBuilder.ToString();
        }

        /// <inheritdoc />
        public string UserAgent { get; }
    }
}
