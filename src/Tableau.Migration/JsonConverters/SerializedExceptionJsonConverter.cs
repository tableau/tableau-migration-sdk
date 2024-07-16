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
using Tableau.Migration.JsonConverters.SerializableObjects;


namespace Tableau.Migration.JsonConverters
{
    /// <summary>
    /// JsonConverter that de/serializes a <see cref="SerializableException"/>. 
    /// </summary>
    public class SerializedExceptionJsonConverter : JsonConverter<SerializableException>
    {
        internal static string GetNamespace(string fullTypeName)
        {
            if (string.IsNullOrEmpty(fullTypeName))
            {
                throw new ArgumentException("The type name cannot be null or empty.", nameof(fullTypeName));
            }

            int lastDotIndex = fullTypeName.LastIndexOf('.');
            if (lastDotIndex == -1)
            {
                throw new ArgumentException("The type name does not contain a namespace.", nameof(fullTypeName));
            }

            return fullTypeName.Substring(0, lastDotIndex);
        }

        /// <summary>
        /// Reads a <see cref="SerializableException"/> from JSON.
        /// </summary>
        /// <param name="reader">The <see cref="Utf8JsonReader"/> to read from.</param>
        /// <param name="typeToConvert">The type of the object to convert.</param>
        /// <param name="options">The <see cref="JsonSerializerOptions"/> to use for deserialization.</param>
        /// <returns>The deserialized <see cref="SerializableException"/>.</returns>
        public override SerializableException? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            SerializableException? ret = null;

            // Check if the reader has never been read from
            if (reader.TokenStartIndex == 0 && reader.CurrentDepth == 0)
            {
                reader.Read();
            }

            JsonReaderUtils.AssertStartObject(ref reader);

            while (reader.Read())
            {
                // Make sure it starts with "ClassName"
                JsonReaderUtils.AssertPropertyName(ref reader, Constants.CLASS_NAME);

                // Read the type of exception class this is
                reader.Read();

                string exceptionTypeStr = reader.GetString() ?? "";

                string exceptionNamespace = GetNamespace(exceptionTypeStr);

                Type? exceptionType = null;
                if (exceptionNamespace == "System")
                {
                    exceptionType = Type.GetType($"{exceptionTypeStr}");
                }
                else
                {
                    exceptionType = Type.GetType($"{exceptionTypeStr}, {exceptionNamespace}");
                }

                // Check if this is a built-in exception type
                if (exceptionType is null)
                {
                    // exception type is not a built in type, looking through Tableau.Migration.dll
                    exceptionType = typeof(FailedJobException).Assembly.GetType(exceptionTypeStr);
                    if (exceptionType is null)
                    {
                        if (exceptionTypeStr == "Python.Runtime.PythonException")
                        {
                            exceptionType = typeof(Python.Runtime.PythonException);
                        }
                        else if (exceptionType != typeof(Python.Runtime.PythonException))
                        {
                            throw new InvalidOperationException($"Unable to get type of '{exceptionTypeStr}'");
                        }

                    }
                }

                // Make sure the next property is the Exception 
                JsonReaderUtils.ReadAndAssertPropertyName(ref reader, Constants.EXCEPTION);

                // Deserialize the exception
                reader.Read();
                var ex = JsonSerializer.Deserialize(ref reader, exceptionType, options) as Exception;

                Guard.AgainstNull(ex, nameof(ex));

                ret = new SerializableException(ex);

                JsonReaderUtils.AssertEndObject(ref reader);
                reader.Read();
                break;
            }

            return ret;
        }

        /// <summary>
        /// Writes a <see cref="SerializableException"/> to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write to.</param>
        /// <param name="value">The <see cref="SerializableException"/> to write.</param>
        /// <param name="options">The <see cref="JsonSerializerOptions"/> to use for serialization.</param>
        public override void Write(Utf8JsonWriter writer, SerializableException value, JsonSerializerOptions options)
        {
            Guard.AgainstNullOrEmpty(value.ClassName, nameof(value.ClassName));
            Guard.AgainstNull(value.Error, nameof(value.Error));

            // Start our serialized Exception object
            writer.WriteStartObject();

            // Save the type of exception it is
            writer.WriteString(Constants.CLASS_NAME, value.ClassName);

            // Save the exception itself
            writer.WritePropertyName(Constants.EXCEPTION);
            JsonSerializer.Serialize(writer, value.Error, value.Error.GetType(), options);

            // End of serialized exception object
            writer.WriteEndObject();
        }
    }
}
