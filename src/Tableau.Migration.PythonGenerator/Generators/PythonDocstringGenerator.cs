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

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.CodeAnalysis;

namespace Tableau.Migration.PythonGenerator.Generators
{
    internal sealed class PythonDocstringGenerator : IPythonDocstringGenerator
    {
        [return: NotNullIfNotNull(nameof(s))]
        private static string? CleanText(string? s)
        {
            if (s is null)
            {
                return null;
            }

            var result = Regex.Replace(s.Trim().ReplaceLineEndings(""), @"\s+", " ");

            if (result.Contains("<see"))
            {
                result = Regex.Replace(result, @"\<see\s+cref=\""(?:\w+\:)(?:.+[.])+(?<type>\w+)(?:`\d)*\""\s+/\>", "$1");
                result = Regex.Replace(result, @"\<see\s+href=\""(?<address>.*)\""\s*\>(?<text>.+)\</see\>", "$1");
            }

            if (result.Contains("<paramref"))
            {
                result = Regex.Replace(result, @"\<paramref\s+name=\""(?<type>\w+)\""\s+/\>", "$1");
            }

            if (result.Contains("para>"))
            {
                result = Regex.Replace(result, @"\<(/*)para\>", string.Empty);
            }

            return result.Trim();
        }

        private static PythonDocstring FromXml(string xmlDoc, bool ignoreArgs)
        {
            var xml = new XmlDocument();
            xml.LoadXml(xmlDoc);

            string summary = string.Empty;
            var summaryEls = xml.DocumentElement?.GetElementsByTagName("summary");
            if (summaryEls is not null && summaryEls.Count > 0)
            {
                summary = CleanText(summaryEls[0]?.InnerXml) ?? string.Empty;
            }

            string? returnDoc = null;
            var returnsEls = xml.DocumentElement?.GetElementsByTagName("returns");
            if (returnsEls is not null && returnsEls.Count > 0)
            {
                returnDoc = CleanText(returnsEls[0]?.InnerXml);
            }

            var args = ImmutableArray.CreateBuilder<PythonArgDocstring>();

            if(!ignoreArgs)
            {
                var argEls = xml.DocumentElement?.GetElementsByTagName("param");
                if (argEls is not null && argEls.Count > 0)
                {
                    for (int i = 0; i < argEls.Count; i++)
                    {
                        var arg = argEls[i];
                        var name = arg?.Attributes?["name"]?.Value?.ToSnakeCase();
                        if (arg is null || name is null)
                        {
                            continue;
                        }

                        args.Add(new(name, CleanText(arg.InnerXml)));
                    }
                }
            }

            return new(summary, args.ToImmutable(), returnDoc);
        }

        public PythonDocstring? Generate(ISymbol dotNetSymbol, bool ignoreArgs = false)
        {
            var xml = dotNetSymbol.GetDocumentationCommentXml();
            if (string.IsNullOrEmpty(xml))
            {
                return null;
            }

            return FromXml(xml, ignoreArgs);
        }
    }
}
