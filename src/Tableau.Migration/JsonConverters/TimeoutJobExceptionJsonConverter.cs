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
using System.Text.Json;
using System.Text.Json.Serialization;
using Tableau.Migration.Api.Models;

namespace Tableau.Migration.JsonConverters
{
    /// <summary>
    /// JsonConverter that serializes a <see cref="TimeoutJobException"/>. It does not support reading exceptions back in.
    /// </summary>
    internal class TimeoutJobExceptionJsonConverter : JsonConverter<TimeoutJobException>
    {
        public TimeoutJobExceptionJsonConverter()
        { }

        public override void Write(Utf8JsonWriter writer, TimeoutJobException value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            JsonWriterUtils.WriteExceptionProperties(ref writer, value);

            // Serialize the Job property if it's not null
            if (value.Job != null)
            {
                writer.WritePropertyName("Job");
                JsonSerializer.Serialize(writer, value.Job, options);
            }

            writer.WriteEndObject();
        }

        public override TimeoutJobException? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            IJob? job = null;
            string? message = null;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var propertyName = reader.GetString();
                    reader.Read(); // Move to the property value.

                    if (propertyName == "Job")
                    {
                        job = JsonSerializer.Deserialize<IJob>(ref reader, options);
                    }
                    else if(propertyName == "Message")
                    {
                        message = reader.GetString();
                    }
                }
                else if (reader.TokenType == JsonTokenType.EndObject)
                {
                    break; // End of the object.
                }
            }

            Guard.AgainstNull(message, nameof(message));       // Message could be an empty string, so just check null

            return new TimeoutJobException(job, message);
        }


    }
}
