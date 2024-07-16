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
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.JsonConverters.SerializableObjects;

namespace Tableau.Migration.JsonConverters
{
    /// <summary>
    /// Represents a collection of serializable entries, organized by a string key and a list of <see cref="SerializableManifestEntry"/> as the value.
    /// This class extends <see cref="Dictionary{TKey, TValue}"/> to facilitate serialization and deserialization of migration manifest entries.
    /// </summary>
    public class RestExceptionJsonConverter : JsonConverter<RestException>
    {
        /// <summary>
        /// Reads and converts the JSON to type <see cref="RestException"/>.
        /// </summary>
        /// <param name="reader">The reader to deserialize objects or value types.</param>
        /// <param name="typeToConvert">The type of object to convert.</param>
        /// <param name="options">Options to control the behavior during reading.</param>
        /// <returns>A <see cref="RestException"/> object deserialized from JSON.</returns>
        public override RestException Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            HttpMethod? httpMethod = null;
            Uri? requestUri = null;
            string? code = null;
            string? detail = null;
            string? summary = null;
            string? exceptionMessage = null;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var propertyName = reader.GetString();
                    reader.Read(); // Move to the property value.

                    switch (propertyName)
                    {
                        case nameof(RestException.HttpMethod):
                            var method = reader.GetString();
                            if (method != null)
                            {
                                httpMethod = new HttpMethod(method);
                            }
                            break;

                        case nameof(RestException.RequestUri):
                            var uriString = reader.GetString();
                            if (uriString != null)
                            {
                                requestUri = new Uri(uriString);
                            }
                            break;

                        case nameof(RestException.Code):
                            code = reader.GetString();
                            break;

                        case nameof(RestException.Detail):
                            detail = reader.GetString();
                            break;

                        case nameof(RestException.Summary):
                            summary = reader.GetString();
                            break;

                        case nameof(RestException.Message):
                            exceptionMessage = reader.GetString();
                            break;
                    }
                }
                else if (reader.TokenType == JsonTokenType.EndObject)
                {
                    break; // End of the object.
                }
            }

            Guard.AgainstNull(exceptionMessage, nameof(exceptionMessage));

            // Use the internal constructor for deserialization
            return new RestException(httpMethod, requestUri, new Error { Code = code, Detail = detail, Summary = summary }, exceptionMessage);
        }

        /// <summary>
        /// Writes a specified <see cref="RestException"/> object to JSON.
        /// </summary>
        /// <param name="writer">The writer to serialize objects or value types.</param>
        /// <param name="value">The <see cref="RestException"/> value to serialize.</param>
        /// <param name="options">Options to control the behavior during writing.</param>
        public override void Write(Utf8JsonWriter writer, RestException value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            if (value.HttpMethod != null)
            {
                writer.WriteString(nameof(RestException.HttpMethod), value.HttpMethod.Method);
            }

            if (value.RequestUri != null)
            {
                writer.WriteString(nameof(RestException.RequestUri), value.RequestUri.ToString());
            }

            if (value.Code != null)
            {
                writer.WriteString(nameof(RestException.Code), value.Code);
            }

            if (value.Detail != null)
            {
                writer.WriteString(nameof(RestException.Detail), value.Detail);
            }

            if (value.Summary != null)
            {
                writer.WriteString(nameof(RestException.Summary), value.Summary);
            }

            JsonWriterUtils.WriteExceptionProperties(ref writer, value);

            writer.WriteEndObject();
        }
    }
}
