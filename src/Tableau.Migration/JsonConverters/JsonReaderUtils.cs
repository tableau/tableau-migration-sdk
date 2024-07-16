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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Tableau.Migration.JsonConverters
{
    internal static class JsonReaderUtils
    {
        /// <summary>
        /// Verify that last json token was a <see cref="JsonTokenType.PropertyName"/>
        /// </summary>
        internal static void AssertPropertyName(ref Utf8JsonReader reader, string? expected = null)
        {
            if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException("Expected property name");
            if (expected != null)
            {
                var actual = reader.GetString();
                if (!string.Equals(actual, expected, StringComparison.Ordinal))
                {
                    throw new JsonException($"Property value did not match expectation. Expected '{expected}' but got '{actual}'");
                }
            }
        }

        /// <summary>
        /// Verify that last json token was a <see cref="JsonTokenType.StartArray"/>
        /// </summary>
        internal static void AssertStartArray(ref Utf8JsonReader reader)
        {
            if (reader.TokenType != JsonTokenType.StartArray) throw new JsonException("Expected start of array");
        }

        /// <summary>
        /// Verify that last json token was a <see cref="JsonTokenType.EndArray"/>
        /// </summary>
        internal static void AssertEndArray(ref Utf8JsonReader reader)
        {
            if (reader.TokenType != JsonTokenType.EndArray) throw new JsonException("Expected end of array");
        }

        /// <summary>
        /// Verify that last json token was a <see cref="JsonTokenType.StartObject"/>
        /// </summary>
        internal static void AssertStartObject(ref Utf8JsonReader reader)
        {
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected start of object");
        }

        /// <summary>
        /// Verify that last json token was a <see cref="JsonTokenType.EndObject"/>
        /// </summary>
        internal static void AssertEndObject(ref Utf8JsonReader reader)
        {
            if (reader.TokenType != JsonTokenType.EndObject) throw new JsonException("Expected end of object");
        }

        /// <summary>
        /// Read the next json token and verify that it's a <see cref="JsonTokenType.PropertyName"/>
        /// </summary>
        internal static void ReadAndAssertPropertyName(ref Utf8JsonReader reader, string? expected = null)
        {
            reader.Read();
            AssertPropertyName(ref reader, expected);
        }

        /// <summary>
        /// Read the next json token and verify that it's a <see cref="JsonTokenType.StartArray"/>
        /// </summary>
        internal static void ReadAndAssertStartArray(ref Utf8JsonReader reader)
        {
            reader.Read();
            AssertStartArray(ref reader);
        }

        /// <summary>
        /// Read the next json token and verify that it's a <see cref="JsonTokenType.StartObject"/>
        /// </summary>
        internal static void ReadAndAssertStartObject(ref Utf8JsonReader reader)
        {
            reader.Read();
            AssertStartObject(ref reader);
        }
    }
}
