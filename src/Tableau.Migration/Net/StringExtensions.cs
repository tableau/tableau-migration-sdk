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
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Xml;
using System.Xml.Serialization;
using Tableau.Migration.Api.ClientXml.Models;

namespace Tableau.Migration.Net
{
    internal static class StringExtensions
    {
        internal static ConcurrentDictionary<Tuple<string, string>, XmlSerializer> _xmlSerializers = new();

        internal static string TrimPaths(this string path) => path.Trim('/');

        internal static string TrimStartPath(this string path) => path.TrimStart('/');

        [return: NotNullIfNotNull(nameof(value))]
        internal static string? UrlEncode(this string? value) => WebUtility.UrlEncode(value);

        [return: NotNullIfNotNull(nameof(value))]
        internal static string? UrlDecode(this string? value) => WebUtility.UrlDecode(value);

        internal static StringContent ToHttpContent(this string content, MediaTypeWithQualityHeaderValue contentType)
        {
#if NET7_0_OR_GREATER
            return new StringContent(content, Constants.DefaultEncoding, contentType);
#else
            return new StringContent(content, Constants.DefaultEncoding, contentType.MediaType);
#endif
        }

        internal static T? FromXml<T>(this string xml)
        {
            var entityEscaped = new StringBuilder(xml.Length);

            foreach (char c in xml)
            {
                if (XmlConvert.IsXmlChar(c) || c >= 0x20)
                {
                    entityEscaped.Append(c);
                }
                else
                {
                    // x20 is space.  lower is control characters
                    // Does this violate the XML spec?
                    // https://stackoverflow.com/questions/730133/what-are-invalid-characters-in-xml
                    // You betcha it does!
                    // Does Tableau Server violate the XML spec?
                    // You betcha it does!  TFS 1436863
                    // The entity escape here is still not valid, but it tricks the XmlReader
                    // see https://stackoverflow.com/questions/21509569/xmlreadersettings-checkcharacters-false-doesnt-seem-to-work
                    entityEscaped.Append("&#" + ((int)c).ToString("D2") + ";");
                }
            }
            using var textReader = new StringReader(entityEscaped.ToString());
            using var reader = new XmlTextReader(textReader);
            reader.Namespaces = false;
            reader.MoveToContent();

            XmlAttributeOverrides? attrOverrides = null;
            if (!StringComparer.OrdinalIgnoreCase.Equals("html", reader.LocalName))
            {
                // Override any [XmlRootAttribute] on T with
                // whatever the root element of the actual XML is.
                //
                // This was added to support the WG Server /vizportal/api/clientxml/auth/login
                // endpoint which returns different root XML elements depending on the situation.
                // This allows us to have quasi union types for deserializing these results.
                //
                // We avoid skip this logic for <html> however because we have special handling
                // other places that expect us to fail when attempting to deserialize an HTML response
                // and this could cause us to "successfully" deserialize the <html> causing an unintended
                // result.
                //
                // -JPOEHLS
                attrOverrides = new XmlAttributeOverrides();

                var attrs = new XmlAttributes
                {
                    XmlRoot = new XmlRootAttribute()
                    {
                        ElementName = reader.LocalName,
                        Namespace = reader.NamespaceURI
                    }
                };

                attrOverrides.Add(typeof(T), attrs);
            }

            //We have to reuse XmlSerializer instances due to .NET memory leaks: https://www-jo.se/f.pfleger/memoryleak
            var typeKey = GetSerializerKey<T>(reader.LocalName);

            //   We override the XmlAttribute.XmlRoot.Element name above.
            //   This can produce a small issue, that if the first time a serializer is called AND it is 
            //   called to deserialize a string that is NOT of type T, then the XmlRoot.ElementName
            //   will be set incorrectly. Additionally, because we have to re-use the same XML Serializer because 
            //   of the memory leak issue described below, this will then make deserializations of valid
            //   objects fail. 
            //   To mitigate this, we use both the type and the XmlRoot.ElementName (which is the same as the 
            //   reader.LocalName) as the lookup into our dictionary
            //   - Steffen
            var dictLookup = Tuple.Create(typeKey, reader.LocalName);

            var serializer = _xmlSerializers.GetOrAdd(dictLookup, t => new XmlSerializer(typeof(T), attrOverrides));

            return (T?)serializer.Deserialize(reader);
        }

        internal static T? FromJson<T>(this string json)
        {
            return JsonSerializer.Deserialize<T>(json, JsonOptions.Default);
        }

        private static string GetSerializerKey<T>(string localName)
        {
            var type = typeof(T).FullName;

            //We want these types cached separately by their inner XML types so the different schemas for each are respected
            if (type == typeof(SuccessfulOrFailedLogin).FullName)
            {
                switch (localName)
                {

                    case SuccessfulOrFailedLogin.SUCCESSFUL_LOGIN_LOCAL_NAME:
                        type = typeof(SuccessfulLogin).FullName;
                        break;


                    case SuccessfulOrFailedLogin.FAILED_LOGIN_LOCAL_NAME:
                        type = typeof(FailedLogin).FullName;
                        break;
                }
            }

            return type!;
        }
    }
}
