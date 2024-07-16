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
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tableau.Migration.Api.Simulation.Rest.Net.Responses;

namespace Tableau.Migration.JsonConverters
{
    /// <summary>
    /// JsonConverter that serializes a <see cref="BuildResponseException"/>. It does not support reading exceptions back in.
    /// </summary>
    internal class BuildResponseExceptionJsonConverter : JsonConverter<BuildResponseException>
    {
        public override void Write(Utf8JsonWriter writer, BuildResponseException value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString(nameof(BuildResponseException.StatusCode), value.StatusCode.ToString());
            writer.WriteNumber(nameof(BuildResponseException.SubCode), value.SubCode);
            writer.WriteString(nameof(BuildResponseException.Summary), value.Summary);
            writer.WriteString(nameof(BuildResponseException.Detail), value.Detail);
            JsonWriterUtils.WriteExceptionProperties(ref writer, value);
            writer.WriteEndObject();
        }

        
        public override BuildResponseException? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            JsonReaderUtils.AssertStartObject(ref reader);

            HttpStatusCode? statusCode = null;
            int? subCode = null;
            string? summary = null;
            string? detail = null;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    string? propertyName = reader.GetString();
                    Guard.AgainstNullOrEmpty(propertyName, nameof(propertyName));

                    reader.Read(); // Move to the property value.

                    switch(propertyName)
                    {
                        case nameof(BuildResponseException.StatusCode):
                            var statusCodeStr = reader.GetString();
                            Guard.AgainstNull(statusCodeStr, nameof(statusCodeStr));
                            statusCode = (HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), statusCodeStr);
                            break;

                        case nameof(BuildResponseException.SubCode):
                            subCode = reader.GetInt32();
                            break;

                        case nameof(BuildResponseException.Summary):
                            summary = reader.GetString();
                            Guard.AgainstNull(summary, nameof(summary));
                            break;

                        case nameof(BuildResponseException.Detail):
                            detail = reader.GetString();
                            Guard.AgainstNull(detail, nameof(detail));
                            break;
                        
                        default:
                            break;
                    }
                }
                else if (reader.TokenType == JsonTokenType.EndObject)
                {
                    break; // End of the object.
                }
            }

            return new BuildResponseException(statusCode!.Value, subCode!.Value, summary!, detail!);
        }

    }
}
