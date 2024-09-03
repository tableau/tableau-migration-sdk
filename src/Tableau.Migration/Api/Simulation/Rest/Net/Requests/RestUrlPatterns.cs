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
using System.Text.RegularExpressions;
using Tableau.Migration.Net;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Requests
{
    /// <summary>
    /// Class containing REST URL regular expressions for HTTP request matching. See tests for example usage.
    /// </summary>
    internal static class RestUrlPatterns
    {
        public const string VersionGroupName = "restApiVersion";

        public const string GuidPattern = "[0-9A-F]{8}[-]?(?:[0-9A-F]{4}[-]?){3}[0-9A-F]{12}";

        public const string ContentUrlPattern = @"[\w-]*";

        private static string IdPattern(string groupName) => $"""(?<{groupName}>{GuidPattern})""";

        public const string SiteIdGroupName = "siteId";
        public static readonly string SiteId = IdPattern(SiteIdGroupName);

        public const string EntityIdGroupName = "entityId";
        public const string NamePattern = @"[a-zA-Z0-9-_]*$";

        public static readonly string EntityId = IdPattern(EntityIdGroupName);

        public static Regex RestApiUrl(string suffix, bool useExperimental = false)
        {
            string pattern;

            if (useExperimental)
            {
                pattern = $"""^/api/(?<{VersionGroupName}>{ApiClient.EXPERIMENTAL_API_VERSION})/{suffix.TrimPaths()}/?$""";
            }
            else
            {
                pattern = $"""^/api/(?<{VersionGroupName}>\d+.\d+)/{suffix.TrimPaths()}/?$""";

            }

            return new(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        public static Regex SiteUrl(string postSiteSuffix, bool useExperimental = false) 
            => RestApiUrl($"""/sites/{SiteId}/{postSiteSuffix.TrimPaths()}""", useExperimental);

        public static Regex EntityUrl(string preEntitySuffix) => RestApiUrl($"""{preEntitySuffix.TrimPaths()}/{EntityId}""");

        public static Regex SiteEntityUrl(
            string postSitePreEntitySuffix,
            string? postEntitySuffix = null,
            bool useExperimental = false)
        {
            var trimmedSuffix = postEntitySuffix?.TrimPaths();
            trimmedSuffix = string.IsNullOrEmpty(trimmedSuffix) ? string.Empty : $"/{trimmedSuffix}";

            return SiteUrl($"""{postSitePreEntitySuffix.TrimPaths()}/{EntityId}{trimmedSuffix}""", useExperimental);
        }

        public static Regex SiteEntityTagsUrl(string postSitePreEntitySuffix, string? postTagsSuffix = null) 
            => SiteEntityUrl(postSitePreEntitySuffix, $"tags/{postTagsSuffix}".TrimEnd('/'));

        public static Regex SiteEntityTagUrl(string postSitePreEntitySuffix) 
            => SiteEntityTagsUrl(postSitePreEntitySuffix, new Regex(NamePattern, RegexOptions.IgnoreCase).ToString());

        public static IEnumerable<(string Key, Regex ValuePattern)> SiteCommitFileUploadQueryString(string typeParam)
        {
            return new List<(string Key, Regex ValuePattern)>
            {
                ("uploadSessionId", new(GuidPattern,RegexOptions.IgnoreCase)),
                (typeParam, new(NamePattern))
            };
        }
    }
}
