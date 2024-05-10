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

namespace Tableau.Migration
{
    internal static class CsvExtensions
    {
        /// <summary>
        /// Creates a csv string from an array of strings in the order passed.
        /// </summary>
        /// <param name="builder">A string builder to append to.</param>
        /// <param name="values">The input string values in order.</param>
        internal static StringBuilder AppendCsvLine(this StringBuilder builder, params string?[] values)
        {
            if (!values.IsNullOrEmpty())
            {
                if (values.Length == 1)
                {
                    builder.AppendCsvValue(values[0]);
                }
                else
                {
                    foreach (var value in values[0..^1])
                    {
                        builder.AppendCsvValue(value);
                        builder.Append(',');
                    }

                    builder.AppendCsvValue(values[^1]);
                }
            }

            builder.AppendLine();
            return builder;
        }

        /// <summary>
        /// Escapes CSV unsafe characters.
        /// </summary>
        /// <param name="builder">A string builder to append to.</param>
        /// <param name="value">The string value to be escaped.</param>
        internal static StringBuilder AppendCsvValue(this StringBuilder builder, string? value)
        {
            if (!value.IsNullOrEmpty() &&
                (value.Contains(',') || value.Contains('"') || value.Contains('\r') || value.Contains('\n')))
            {
                builder.Append('"');
                builder.Append(value.Replace("\"", "\"\""));
                builder.Append('"');
            }
            else
            {
                builder.Append(value);
            }

            return builder;
        }
    }
}
