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
using System.Linq;

namespace Tableau.Migration.Net
{
    internal sealed class QueryStringBuilder : IQueryStringBuilder
    {
        private readonly Dictionary<string, string> _query = new();

        /// <inheritdoc/>
        public bool IsEmpty => _query.Count == 0;

        /// <inheritdoc/>
        public string Build()
        {
            return String.Join("&", _query.Select(p => $"{p.Key.UrlEncode()}={p.Value.UrlEncode()}"));
        }

        /// <inheritdoc/>
        public void AddOrUpdate(string query)
        {
            var parts = query.Split('=', 2);

            AddOrUpdate(parts[0].Trim(), parts[1].Trim());
        }

        /// <inheritdoc/>
        public void AddOrUpdate(IDictionary<string, string> items)
        {
            foreach (var item in items)
                AddOrUpdate(item.Key, item.Value);
        }

        /// <inheritdoc/>
        public void AddOrUpdate(string key, string value)
        {
            _query[key] = value;
        }
    }
}
