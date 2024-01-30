// Copyright (c) 2023, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the ""License"") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an ""AS IS"" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using System;
using System.Reflection;

namespace Tableau.Migration
{
    /// <summary>
    /// Implementation responsible to return the current SDK version and user agent string, based on the Executing Assembly Version.
    /// </summary>
    internal sealed class MigrationSdk : IMigrationSdk
    {
        private readonly IUserAgentSuffixProvider _userAgentSuffixProvider;

        /// <summary>
        /// Sets the <see cref="MigrationSdk"/> properties on initialization so they are immutable.
        /// </summary>
        public MigrationSdk(IUserAgentSuffixProvider userAgentSuffixProvider)
        {
            Version = Assembly.GetExecutingAssembly().GetName().Version ?? new Version();
            _userAgentSuffixProvider = userAgentSuffixProvider;

            UserAgent = $"{Constants.USER_AGENT_PREFIX}{_userAgentSuffixProvider.UserAgentSuffix}/{Version}";
        }
        /// <summary>
        /// The current SDK Version
        /// </summary>
        /// <returns>The current SDK version.</returns>
        public Version Version { get; init; }
        /// <summary>
        /// Identifier string for the SDK user-agent.
        /// </summary>

        public string UserAgent { get; init; }
    }
}
