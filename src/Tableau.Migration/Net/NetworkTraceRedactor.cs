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
using System.Text.RegularExpressions;

namespace Tableau.Migration.Net
{
    internal class NetworkTraceRedactor
        : INetworkTraceRedactor
    {
        private static readonly Regex[] SENSITIVE_TEXT_REGEXES = new[]
        {
            // Xml Element secrets
            new Regex(
                "<(authenticity_token|modulus|exponent)>(?<SENSITIVE_VALUE>.*)</(\\1)>",
                RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase),
            // Xml Attribute secrets
            new Regex(
                "(password|token|personalAccessTokenSecret|jwt)=\"(?<SENSITIVE_VALUE>[^\"]*)\"",
                RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase)
        };

        private static readonly string[] SENSITIVE_CONTENT_HEADER_NAMES = new[]
        {
            "password",
            "token",
            "authenticity_token",
            "full_keychain_key",
            "crypted"
        };

        internal const string SENSITIVE_DATA_PLACEHOLDER = "*****";

        public string ReplaceSensitiveData(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return input;
            }

            foreach (var regex in SENSITIVE_TEXT_REGEXES)
            {
                if (!regex.IsMatch(input))
                {
                    continue;
                }

                input = regex.Replace(input, m => (m.Groups["SENSITIVE_VALUE"].Success && !m.Groups["SENSITIVE_VALUE"].Value.IsNullOrEmpty())
                    ? m.Value.Replace(m.Groups["SENSITIVE_VALUE"].Value, SENSITIVE_DATA_PLACEHOLDER, StringComparison.Ordinal)
                    : m.Value);
            }

            return input;
        }

        public bool IsSensitiveMultipartContent(
            string? contentName)
        {
            if (string.IsNullOrWhiteSpace(contentName))
            {
                return false;
            }

            return SENSITIVE_CONTENT_HEADER_NAMES
                .Any(sensitiveName =>
                    contentName
                        .Trim()
                        .Contains(
                            sensitiveName,
                            StringComparison.Ordinal));
        }
    }
}
