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
using Python.Runtime;

namespace Tableau.Migration.JsonConverters
{
    /// <summary>
    /// JsonConverter that serializes a <see cref="PythonException"/>.
    /// </summary>
    public class PythonExceptionConverter : JsonConverter<PythonException>
    {
        // **NOTE**
        // I am not convinced this works. 
        // I've tried to create a PythonException manually to pass into the write and read method, but I can't create one.
        // Every time I try I get an exception through in the Python.NET layer I get an exception thrown in the Python.NET layer.
        //
        // The other option is to actually run python code that will produce an PythonException, except that requires
        // scaffolding as it requires the python library to be imported to PythonRuntime.
        // See: https://github.com/pythonnet/pythonnet#embedding-python-in-net
        //     You must set Runtime.PythonDLL property or PYTHONNET_PYDLL environment variable starting with version 3.0,
        //     otherwise you will receive BadPythonDllException (internal, derived from MissingMethodException) upon calling Initialize.
        //     Typical values are python38.dll (Windows), libpython3.8.dylib (Mac), libpython3.8.so (most other Unix-like operating systems).
        // This is difficult because I have no way to get a libpython.<version> on mac. 
        // I could do this for just windows, but that would require us to add a python38.dll to the repo.
        // 
        // I have tested that the "write" works manually. This is because I had a bug in the Python.TestApplication in a hook, that produced it.
        // I have no idea how it actually made it into the manifest though, and at this point, I'm out of time. 
        // I have opened a user story to work on this some more at a later time.

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The reader to read from.</param>
        /// <param name="typeToConvert">The type of the object.</param>
        /// <param name="options">The serializer options.</param>
        /// <returns>The deserialized object.</returns>
        public override PythonException? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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
                        Guard.AgainstNull(message, nameof(message));       // Message could be an empty string, so just check null
                    }
                }
                else if (reader.TokenType == JsonTokenType.EndObject)
                {
                    break; // End of the object.
                }
            }

            // Message must be deserialized by now
            Guard.AgainstNull(message, nameof(message));
            var ret = new PythonException(PyType.Get(typeof(string)), PyType.None, null, message, null);
            return ret;
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The writer to write to.</param>
        /// <param name="value">The object to write.</param>
        /// <param name="options">The serializer options.</param>
        public override void Write(Utf8JsonWriter writer, PythonException value, JsonSerializerOptions options)
        {
            if (value is PythonException pyException)
            {
                writer.WriteStartObject();
                writer.WriteString("ClassName", typeof(PythonException).FullName);
                writer.WriteString("Message", pyException.Format());
                writer.WriteEndObject();
                return;
            }
        }
    }
}
