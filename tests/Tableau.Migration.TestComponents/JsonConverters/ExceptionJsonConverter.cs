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

using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tableau.Migration.TestComponents.JsonConverters
{
    // Source: https://code-maze.com/dotnet-serialize-exceptions-as-json/
    /// <summary>
    /// JsonConverter that serializes a <see cref="Exception"/>. It does not support reading exceptions back in.
    /// </summary>
    public class ExceptionJsonConverter : JsonConverter<Exception>
    {
        ILogger<ExceptionJsonConverter> _logger;

        public ExceptionJsonConverter(ILogger<ExceptionJsonConverter> logger)
        {
            _logger = logger;
        }

        public override Exception? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Reading exception back in is not supported.
            reader.TrySkip();
            return null;
        }

        public override void Write(Utf8JsonWriter writer, Exception value, JsonSerializerOptions options)
        {
            var exceptionType = value.GetType();

            var properties = exceptionType.GetProperties()
                .Where(e => e.PropertyType != typeof(Type))
                .Where(e => e.PropertyType.Namespace != typeof(MemberInfo).Namespace)
                .ToList();

            writer.WriteStartObject();
            writer.WriteString("ClassName", exceptionType.FullName);
            foreach (var property in properties)
            {
                // We can't write cancellation tokens because they are disposed by the time we get here.
                if(property.PropertyType == typeof(CancellationToken))
                    continue;

                try
                {
                    var propertyValue = property.GetValue(value, null);

                    if (propertyValue is null)
                        continue;

                    writer.WritePropertyName(property.Name);

                    JsonSerializer.Serialize(writer, propertyValue, property.PropertyType, options);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unable to write property");
                }
            }

            writer.WriteEndObject();
        }
    }
}
