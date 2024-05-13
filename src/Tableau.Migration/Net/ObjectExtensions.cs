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

using System.IO;
using System.Text.Json;
using System.Xml;
using System.Xml.Serialization;

namespace Tableau.Migration.Net
{
    internal static class ObjectExtensions
    {
        public static string ToXml<T>(this T obj)
        {
            var ns = new XmlSerializerNamespaces(new[] { new XmlQualifiedName("", "") });

            var settings = new XmlWriterSettings()
            {
                OmitXmlDeclaration = true,
                Encoding = Constants.DefaultEncoding
            };

            var serializer = new XmlSerializer(obj?.GetType() ?? typeof(T));

            using var stringWriter = new StringWriter();
            using var xmlWriter = XmlWriter.Create(stringWriter, settings);

            serializer.Serialize(xmlWriter, obj, ns);

            return stringWriter.ToString();
        }

        public static string ToJson<T>(
            this T obj)
            => JsonSerializer.Serialize(
                obj,
                JsonOptions.Default);
    }
}
