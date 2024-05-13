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
using System.Collections.Immutable;
using System.Linq;
using System.Net.Http;
using System.Web;
using Tableau.Migration.Net.Rest.Paging;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Responses
{
    internal sealed class RestEntityResponsePager<TEntity> : IEntityResponsePager<TEntity>
    {
        private const int _defaultPageNumber = 1; //Pages are one-indexed (boo).
        private const int _defaultPageSize = 100;

        public Page GetPageOptions(HttpRequestMessage request)
        {
            int pageNumber = _defaultPageNumber;
            int pageSize = _defaultPageSize;

            if (request.RequestUri is not null)
            {
                var query = HttpUtility.ParseQueryString(request.RequestUri.Query);

                var pageNumberValue = query[PageBuilder.PageNumberKey];
                if (!string.IsNullOrEmpty(pageNumberValue))
                    pageNumber = int.Parse(pageNumberValue);

                var pageSizeValue = query[PageBuilder.PageSizeKey];
                if (!string.IsNullOrEmpty(pageSizeValue))
                    pageSize = int.Parse(pageSizeValue);
            }

            return new(pageNumber, pageSize);
        }

        public ImmutableArray<TEntity> GetPage(IEnumerable<TEntity> entities, Page pageOptions)
        {
            return entities.Skip((pageOptions.PageNumber - 1) * pageOptions.PageSize) //Pages are one-indexed.
                .Take(pageOptions.PageSize)
                .ToImmutableArray();
        }
    }
}
