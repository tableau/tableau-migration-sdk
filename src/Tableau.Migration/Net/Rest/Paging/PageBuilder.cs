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

using System.Collections.Generic;
using System.Linq;

namespace Tableau.Migration.Net.Rest.Paging
{
    /// <summary>
    /// <para>
    /// Class that can build REST API field query strings.
    /// </para>
    /// <para>
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_concepts_fields.htm">Tableau API Reference</see> for more details.
    /// </para>
    /// </summary>
    internal sealed class PageBuilder : RestQueryBuilderBase<Page>, IPageBuilder
    {
        internal const string PageSizeKey = "pageSize";
        internal const string PageNumberKey = "pageNumber";

        /// <inheritdoc/>
        public IPageBuilder SetPage(Page page)
        {
            _items.Clear();
            _items.Add(page);
            return this;
        }

        /// <inheritdoc/>
        protected override IDictionary<string, string>? BuildQueryString()
        {
            if (IsEmpty)
                return null;

            var page = _items.First();

            return new Dictionary<string, string>
            {
                [PageSizeKey] = page.PageSize.ToString(),
                [PageNumberKey] = page.PageNumber.ToString()
            };
        }
    }
}
