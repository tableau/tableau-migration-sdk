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

using System;
using System.Text.Json;

namespace Tableau.Migration.JsonConverters
{
    internal sealed class TableauInstanceTypeNotSupportedExceptionJsonConverter : ExceptionJsonConverter<TableauInstanceTypeNotSupportedException>
    {
        /// <inheritdoc />
        protected override void WriteExtraExceptionProperties(Utf8JsonWriter writer, TableauInstanceTypeNotSupportedException value, JsonSerializerOptions options)
        {
            writer.WritePropertyName(nameof(TableauInstanceTypeNotSupportedException.UnsupportedInstanceType));
            JsonSerializer.Serialize(writer, value.UnsupportedInstanceType, options);
        }

        public override TableauInstanceTypeNotSupportedException? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            TableauInstanceType? unsupportedInstanceType = null;
            string? message = null;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var propertyName = reader.GetString();
                    reader.Read(); // Move to the property value.

                    if (propertyName == nameof(TableauInstanceTypeNotSupportedException.UnsupportedInstanceType))
                    {
                        unsupportedInstanceType = JsonSerializer.Deserialize<TableauInstanceType>(ref reader, options);
                    }
                    else if (propertyName == nameof(Exception.Message))
                    {
                        message = reader.GetString();
                    }
                }
                else if (reader.TokenType == JsonTokenType.EndObject)
                {
                    break; // End of the object.
                }
            }

            Guard.AgainstNull(unsupportedInstanceType, nameof(unsupportedInstanceType));
            Guard.AgainstNull(message, nameof(message));       // Message could be an empty string, so just check null

            return new TableauInstanceTypeNotSupportedException(unsupportedInstanceType.Value, message);
        }
    }
}
