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

namespace Tableau.Migration.Net
{
    /// <summary>
    /// Interface for an object that can serialize to and from common Tableau API formats.
    /// </summary>
    public interface ITableauSerializer
    {
        /// <summary>
        /// Serializes the specified object to an XML string.
        /// </summary>
        /// <typeparam name="T">The type to serialize.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>A serialized string for the specified object.</returns>
        string SerializeToXml<T>(T obj)
            where T : class => obj.ToXml();

        /// <summary>
        /// Serializes the specified object to an JSON string.
        /// </summary>
        /// <typeparam name="T">The type to serialize.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>A serialized string for the specified object.</returns>
        string SerializeToJson<T>(T obj)
            where T : class => JsonSerializer.Serialize(obj, JsonOptions.Default);

        /// <summary>
        /// Deserializes the given XML.
        /// </summary>
        /// <typeparam name="T">The type to deserialize to.</typeparam>
        /// <param name="xml">The XML to deserialize.</param>
        /// <returns>The deserialized <typeparamref name="T"/> instance, or null if it could not be deserialized.</returns>
        T? DeserializeFromXml<T>(string xml) => xml.FromXml<T>();

        /// <summary>
        /// Deserializes the given JSON.
        /// </summary>
        /// <typeparam name="T">The type to deserialize to.</typeparam>
        /// <param name="json">The JSON to deserialize.</param>
        /// <returns>The deserialized <typeparamref name="T"/> instance, or null if it could not be deserialized.</returns>
        T? DeserializeFromJson<T>(string json) => JsonSerializer.Deserialize<T>(json, JsonOptions.Default);

        /// <summary>
        /// Deserializes the given JSON.
        /// </summary>
        /// <typeparam name="T">The type to deserialize to.</typeparam>
        /// <param name="utf8Json">The JSON to deserialize.</param>
        /// <returns>The deserialized <typeparamref name="T"/> instance, or null if it could not be deserialized.</returns>
        T? DeserializeFromJson<T>(ReadOnlySpan<byte> utf8Json) => JsonSerializer.Deserialize<T>(utf8Json, JsonOptions.Default);
    }
}