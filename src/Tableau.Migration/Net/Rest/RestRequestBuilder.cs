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
using Tableau.Migration.Net.Rest.Fields;
using Tableau.Migration.Net.Rest.Filtering;
using Tableau.Migration.Net.Rest.Paging;
using Tableau.Migration.Net.Rest.Sorting;

namespace Tableau.Migration.Net.Rest
{
    internal sealed class RestRequestBuilder : RequestBuilderBase<RestRequestBuilder>, IRestRequestBuilder
    {
        private readonly IFieldBuilder _fields;
        private readonly ISortBuilder _sorts;
        private readonly IFilterBuilder _filters;
        private readonly IPageBuilder _paging;

        private string? _apiVersion;
        private string? _siteId;

        public RestRequestBuilder(
            Uri baseUri,
            string path,
            IHttpRequestBuilderFactory requestBuilderFactory)
            : this(
                baseUri,
                path,
                requestBuilderFactory,
                new QueryStringBuilder(),
                new FieldBuilder(),
                new FilterBuilder(),
                new SortBuilder(),
                new PageBuilder())
        { }

        internal RestRequestBuilder(
            Uri baseUri,
            string path,
            IHttpRequestBuilderFactory requestBuilderFactory,
            IQueryStringBuilder query,
            IFieldBuilder fields,
            IFilterBuilder filters,
            ISortBuilder sorts,
            IPageBuilder paging)
            : base(baseUri, path, requestBuilderFactory, query)
        {
            _fields = fields;
            _filters = filters;
            _sorts = sorts;
            _paging = paging;
        }

        /// <inheritdoc/>
        public IRestRequestBuilder WithApiVersion(string apiVersion)
        {
            _apiVersion = apiVersion;
            return this;
        }

        /// <inheritdoc/>
        public IRestRequestBuilder WithSiteId(Guid siteId)
        {
            _siteId = siteId.ToUrlSegment();
            return this;
        }

        /// <inheritdoc/>
        public IRestRequestBuilder WithSiteId(string? siteId)
        {
            _siteId = siteId;
            return this;
        }

        /// <inheritdoc/>
        public IRestRequestBuilder WithFields(Action<IFieldBuilder> fields)
        {
            fields(_fields);
            return this;
        }

        /// <inheritdoc/>
        public IRestRequestBuilder WithFields(params Field[] fields)
        {
            _fields.AddFields(fields);
            return this;
        }

        /// <inheritdoc/>
        public IRestRequestBuilder WithFilters(Action<IFilterBuilder> filters)
        {
            filters(_filters);
            return this;
        }

        /// <inheritdoc/>
        public IRestRequestBuilder WithFilters(params Filter[] filters)
        {
            _filters.AddFilters(filters);
            return this;
        }

        /// <inheritdoc/>
        public IRestRequestBuilder WithFilters(IEnumerable<Filter> filters)
        {
            _filters.AddFilters(filters);
            return this;
        }

        /// <inheritdoc/>
        public IRestRequestBuilder WithSorts(Action<ISortBuilder> sorts)
        {
            sorts(_sorts);
            return this;
        }

        /// <inheritdoc/>
        public IRestRequestBuilder WithSorts(params Sort[] sorts)
        {
            _sorts.AddSorts(sorts);
            return this;
        }

        /// <inheritdoc/>
        public IRestRequestBuilder WithPage(Action<IPageBuilder> paging)
        {
            paging(_paging);
            return this;
        }

        /// <inheritdoc/>
        public IRestRequestBuilder WithPage(Page page)
        {
            _paging.SetPage(page);
            return this;
        }

        protected override void BuildPath(List<string> segments)
        {
            const string API_SEGMENT = "api";
            const string SITES_SEGMENT = "sites";

            if (!segments.Contains(SITES_SEGMENT, StringComparer.OrdinalIgnoreCase))
            {
                if (_siteId is not null)
                    segments.InsertRange(0, new[] { SITES_SEGMENT, _siteId });
            }

            if (!segments.Contains(API_SEGMENT, StringComparer.OrdinalIgnoreCase))
            {
                if (_apiVersion is not null)
                    segments.InsertRange(0, new[] { API_SEGMENT, _apiVersion });
            }
        }

        protected override void BuildQuery(IQueryStringBuilder query)
        {
            base.BuildQuery(query);

            _fields.AppendQueryString(query);
            _filters.AppendQueryString(query);
            _sorts.AppendQueryString(query);
            _paging.AppendQueryString(query);
        }

        IRestRequestBuilder IRequestBuilder<IRestRequestBuilder>.WithQuery(Action<IQueryStringBuilder> query) => WithQuery(query);

        IRestRequestBuilder IRequestBuilder<IRestRequestBuilder>.WithQuery(string key, string value) => WithQuery(key, value);
    }
}
