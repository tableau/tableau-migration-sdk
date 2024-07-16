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

namespace Tableau.Migration.JsonConverters
{
    /// <summary>
    /// JsonConverter that serializes and deserializes any type of <see cref="Exception"/>.
    /// </summary>
    /// <typeparam name="TException">The type of the exception to convert.</typeparam>
    public class ExceptionJsonConverter<TException> : JsonConverter<TException> where TException : Exception
    {
        /// <summary>
        /// Writes the JSON representation of an Exception object.
        /// </summary>
        /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write to.</param>
        /// <param name="value">The <see cref="Exception"/> object to serialize.</param>
        /// <param name="options">The <see cref="JsonSerializerOptions"/> to use for serialization.</param>
        public override void Write(Utf8JsonWriter writer, TException value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            JsonWriterUtils.WriteExceptionProperties(ref writer, value);
            writer.WriteEndObject();
        }

        /// <summary>
        /// Reads the JSON representation of an Exception object.
        /// </summary>
        /// <param name="reader">The <see cref="Utf8JsonReader"/> to read from.</param>
        /// <param name="typeToConvert">The type of the object to convert.</param>
        /// <param name="options">The <see cref="JsonSerializerOptions"/> to use for deserialization.</param>
        /// <returns>The deserialized <see cref="Exception"/> object.</returns>
        public override TException? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            JsonReaderUtils.AssertStartObject(ref reader);

            string? message = null;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    string? propertyName = reader.GetString();
                    Guard.AgainstNullOrEmpty(propertyName, nameof(propertyName));

                    reader.Read(); // Move to the property value.

                    if (propertyName == "Message")
                    {
                        message = reader.GetString();
                        Guard.AgainstNull(message, nameof(message)); // Message could be an empty string, so just check null
                    }
                    if (propertyName == "InnerException")
                    {
                        reader.Skip();  // Don't read in the inner exceptions
                    }
                }
                else if (reader.TokenType == JsonTokenType.EndObject)
                {
                    break; // End of the object.
                }
            }

            // Message must be deserialized by now
            Guard.AgainstNull(message, nameof(message));

            return (TException)Activator.CreateInstance(typeof(TException), message)!;
        }
    }
}
