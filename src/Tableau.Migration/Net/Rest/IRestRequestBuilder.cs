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
using Tableau.Migration.Net.Rest.Fields;
using Tableau.Migration.Net.Rest.Filtering;
using Tableau.Migration.Net.Rest.Paging;
using Tableau.Migration.Net.Rest.Sorting;

namespace Tableau.Migration.Net.Rest
{
    /// <summary>
    /// Interface for a class that can build REST API requests.
    /// </summary>
    public interface IRestRequestBuilder : IRequestBuilder<IRestRequestBuilder>
    {
        /// <summary>
        /// Sets the URI's version segment, i.e. "/api/{version}".
        /// </summary>
        /// <param name="apiVersion">The REST API version.</param>
        /// <returns>The current <see cref="IRestRequestBuilder"/> instance.</returns>
        IRestRequestBuilder WithApiVersion(string apiVersion);

        /// <summary>
        /// Sets the the URI's sites segment, i.e. "/sites/{site-id}".
        /// </summary>
        /// <param name="siteId">The site's ID.</param>
        /// <returns>The current <see cref="IRestRequestBuilder"/> instance.</returns>
        IRestRequestBuilder WithSiteId(Guid siteId);

        /// <summary>
        /// Sets the the URI's sites segment, i.e. "/sites/{site-id}".
        /// </summary>
        /// <param name="siteId">The site's ID.</param>
        /// <returns>The current <see cref="IRestRequestBuilder"/> instance.</returns>
        IRestRequestBuilder WithSiteId(string? siteId);

        /// <summary>
        /// Configures the fields for the URI.
        /// </summary>
        /// <param name="fields">The callback used to build the URI's fields query string.</param>
        /// <returns>The current <see cref="IRestRequestBuilder"/> instance.</returns>
        IRestRequestBuilder WithFields(Action<IFieldBuilder> fields);

        /// <summary>
        /// Configures the fields for the URI.
        /// </summary>
        /// <param name="fields">The fields used to build the URI's fields query string.</param>
        /// <returns>The current <see cref="IRestRequestBuilder"/> instance.</returns>
        IRestRequestBuilder WithFields(params Field[] fields);

        /// <summary>
        /// Configures the filters for the URI.
        /// </summary>
        /// <param name="filters">The callback used to build the URI's filter query string.</param>
        /// <returns>The current <see cref="IRestRequestBuilder"/> instance.</returns>
        IRestRequestBuilder WithFilters(Action<IFilterBuilder> filters);

        /// <summary>
        /// Configures the filters for the URI.
        /// </summary>
        /// <param name="filters">The filters used to build the URI's filter query string.</param>
        /// <returns>The current <see cref="IRestRequestBuilder"/> instance.</returns>
        IRestRequestBuilder WithFilters(params Filter[] filters);

        /// <summary>
        /// Configures the filters for the URI.
        /// </summary>
        /// <param name="filters">The filters used to build the URI's filter query string.</param>
        /// <returns>The current <see cref="IRestRequestBuilder"/> instance.</returns>
        IRestRequestBuilder WithFilters(IEnumerable<Filter> filters);

        /// <summary>
        /// Configures the page for the URI.
        /// </summary>
        /// <param name="paging">The callback used to build the URI's page query string.</param>
        /// <returns>The current <see cref="IRestRequestBuilder"/> instance.</returns>
        IRestRequestBuilder WithPage(Action<IPageBuilder> paging);

        /// <summary>
        /// Configures the page for the URI.
        /// </summary>
        /// <param name="page">The page used to build the URI's filter query string.</param>
        /// <returns>The current <see cref="IRestRequestBuilder"/> instance.</returns>
        IRestRequestBuilder WithPage(Page page);

        /// <summary>
        /// Configures the page for the URI.
        /// </summary>
        /// <param name="pageNumber">The 1-indexed page number for the page.</param>
        /// <param name="pageSize">The expected maximum number of items to include in the page.</param>
        /// <returns>The current <see cref="IRestRequestBuilder"/> instance.</returns>
        public IRestRequestBuilder WithPage(int pageNumber, int pageSize) => WithPage(new Page(pageNumber, pageSize));

        /// <summary>
        /// Configures the sorts for the URI.
        /// </summary>
        /// <param name="sorts">The callback used to build the URI's sort query string.</param>
        /// <returns>The current <see cref="IRestRequestBuilder"/> instance.</returns>
        IRestRequestBuilder WithSorts(Action<ISortBuilder> sorts);

        /// <summary>
        /// Configures the sorts for the URI.
        /// </summary>
        /// <param name="sorts">The callback used to build the URI's sort query string.</param>
        /// <returns>The current <see cref="IRestRequestBuilder"/> instance.</returns>
        IRestRequestBuilder WithSorts(params Sort[] sorts);
    }
}
