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

using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Tableau.Migration.PythonGenerator
{
    internal static class StringExtensions
    {
        [return: NotNullIfNotNull(nameof(s))]
        public static string? ToSnakeCase(this string? s)
        {
            if(string.IsNullOrEmpty(s))
            {
                return s;
            }

            var sb = new StringBuilder();
            sb.Append(char.ToLowerInvariant(s[0]));
            for (int i = 1; i < s.Length; ++i)
            {
                char c = s[i];
                if (char.IsUpper(c))
                {
                    sb.Append('_');
                    sb.Append(char.ToLowerInvariant(c));
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        [return: NotNullIfNotNull(nameof(s))]
        public static string? ToConstantCase(this string? s)
            => ToSnakeCase(s)?.ToUpper();
    }
}
