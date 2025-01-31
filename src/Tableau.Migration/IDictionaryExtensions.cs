//
//  Copyright (c) 2025, Salesforce, Inc.
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tableau.Migration
{
    internal static class IDictionaryExtensions
    {
        public static string GetContentsAsString<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        {
            if (dictionary.Count == 0)
            {
                return "Empty";
            }

            StringBuilder sb = new StringBuilder();
            foreach (var kvp in dictionary)
            {
                sb.AppendLine($"Key: {kvp.Key}, Value: {kvp.Value}");
            }
            return sb.ToString();
        }

        public static string GetContentsAsString(this IDictionary dictionary)
        {
            if (dictionary.Count == 0)
            {
                return "Empty";
            }

            StringBuilder sb = new StringBuilder();
            foreach (DictionaryEntry entry in dictionary)
            {
                sb.AppendLine($"Key: {entry.Key}, Value: {entry.Value}");
            }
            return sb.ToString();
        }
    }
}
