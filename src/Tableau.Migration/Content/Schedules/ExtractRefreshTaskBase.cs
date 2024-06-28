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
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content.Search;

namespace Tableau.Migration.Content.Schedules
{
    internal abstract class ExtractRefreshTaskBase<TSchedule> :
            ContentBase, IExtractRefreshTask<TSchedule>
        where TSchedule : ISchedule
    {
        public string Type { get; set; }
        public ExtractRefreshContentType ContentType { get; set; }
        public IContentReference Content { get; set; }
        public TSchedule Schedule { get; }

        protected ExtractRefreshTaskBase(
            Guid extractRefreshId,
            string type,
            ExtractRefreshContentType contentType,
            IContentReference content,
            TSchedule schedule)
            : base(
                new ContentReferenceStub(
                    extractRefreshId,
                    string.Empty,
                    new (extractRefreshId.ToString())))
        {
            Type = type;
            ContentType = contentType;
            Content = content;
            Schedule = schedule;
        }

        protected static async Task<TExtractRefreshTask> CreateAsync<TExtractRefreshType, TExtractRefreshTask>(
            TExtractRefreshType? response,
            IContentReferenceFinderFactory finderFactory,
            Func<TExtractRefreshType, IContentReference, CancellationToken, Task<TExtractRefreshTask>> modelFactory,
            CancellationToken cancel)
            where TExtractRefreshType : class, IExtractRefreshType
            where TExtractRefreshTask: IExtractRefreshTask<TSchedule>
        {
            Guard.AgainstNull(response, nameof(response));

            var contentReference = await finderFactory
                .FindExtractRefreshContentAsync(
                    response.GetContentType(),
                    response.GetContentId(),
                    cancel)
                .ConfigureAwait(false);

            var model = await modelFactory(
                    response, 
                    contentReference,
                    cancel)
                .ConfigureAwait(false);

            return model;
        }

        protected static async Task<IImmutableList<TExtractRefreshTask>> CreateManyAsync<TResponse,TExtractRefreshType, TExtractRefreshTask>(
            TResponse? response,
            Func<TResponse, IEnumerable<TExtractRefreshType?>> responseItemFactory,
            Func<TExtractRefreshType, IContentReference, CancellationToken, Task<TExtractRefreshTask>> modelFactory,
            IContentReferenceFinderFactory finderFactory,
            CancellationToken cancel)
            where TResponse : ITableauServerResponse
            where TExtractRefreshType : class, IExtractRefreshType
            where TExtractRefreshTask: IExtractRefreshTask<TSchedule>
        {
            Guard.AgainstNull(response, nameof(response));

            var tasks = ImmutableArray.CreateBuilder<TExtractRefreshTask>();

            var items = responseItemFactory(response).ExceptNulls();

            foreach (var item in items)
            {
                tasks.Add(
                    await CreateAsync(
                        item, 
                        finderFactory, 
                        modelFactory, 
                        cancel)
                    .ConfigureAwait(false));
            }

            return tasks.ToImmutable();
        }
    }
}