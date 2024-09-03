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
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Xunit;

namespace Tableau.Migration.Tests
{
    public static partial class JsonExtensions
    {
        private static readonly Regex IndexRegex = new(@"\[(\d+)\]", RegexOptions.Compiled | RegexOptions.Singleline);

        public static JsonNode? GetByPath(this JsonNode node, string path, [DoesNotReturnIf(true)] bool throwIfNotFound, char pathDelimiter = '.')
        {
            var pathParts = path.Split(pathDelimiter, StringSplitOptions.RemoveEmptyEntries);

            JsonNode? foundNode = null;

            var visitedPaths = new List<string>();

            foreach (var pathPart in pathParts)
            {
                visitedPaths.Add(pathPart);

                var currentNode = foundNode ?? node;

                var indexMatch = IndexRegex.Match(pathPart);

                if (indexMatch.Success)
                {
                    foundNode = GetArrayItem(currentNode, pathPart, indexMatch);
                }
                else
                {
                    foundNode = currentNode.AsObject()[pathPart];
                }

                if (foundNode is null)
                {
                    if (throwIfNotFound)
                        throw new Exception($"Could not find a node at path '{String.Join(pathDelimiter, visitedPaths)}': JSON{Environment.NewLine}{node.ToJsonString()}");
                    else
                        return null;
                }
            }

            return foundNode;

            static JsonNode? GetArrayItem(JsonNode node, string path, Match indexMatch)
            {
                Assert.True(indexMatch.Success);

                var index = Int32.Parse(indexMatch.Groups[1].Value);
                var arrayPath = IndexRegex.Replace(path, String.Empty);

                var arrayNode = node[arrayPath];

                return arrayNode?[index];
            }
        }

        public static JsonNode? GetArrayItemByPath(this JsonNode node, string path, int index, [DoesNotReturnIf(true)] bool throwIfNotFound, char pathDelimiter = '.')
        {
            var foundNode = node.GetByPath(path, throwIfNotFound, pathDelimiter);
            
            var arrayNode = foundNode?.AsArray();

            if (arrayNode is null)
            {
                if (throwIfNotFound)
                    throw new Exception($"The node at path '{path}' is not an array: JSON{Environment.NewLine}{node.ToJsonString()}");
                else
                    return null;
            }

            var arrayItemNode = arrayNode[index];

            if (arrayItemNode is null)
            {
                if (throwIfNotFound)
                    throw new Exception($"Cound not find an array item with index {index} at path '{path}': JSON{Environment.NewLine}{node.ToJsonString()}");
                else
                    return null;
            }

            return arrayItemNode;
        }

        public static void ReplaceWith<T>(this JsonNode node, string path, T value, [DoesNotReturnIf(true)] bool throwIfNotFound, char pathDelimiter = '.')
        {
            var found = node.GetByPath(path, throwIfNotFound, pathDelimiter);

            found?.ReplaceWith(value);
        }
    }
}