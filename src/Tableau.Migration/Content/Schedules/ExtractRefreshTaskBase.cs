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
using Microsoft.Extensions.Logging;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Resources;

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

        protected static async Task<IImmutableList<TExtractRefreshTask>> CreateManyAsync<TResponse, TExtractRefreshType, TExtractRefreshTask>(
            TResponse response,
            Func<TResponse, IEnumerable<TExtractRefreshType?>> responseItemFactory,
            Func<TExtractRefreshType, IContentReference, CancellationToken, Task<TExtractRefreshTask>> modelFactory,
            IContentReferenceFinderFactory finderFactory,
            ILogger logger, ISharedResourcesLocalizer localizer,
            CancellationToken cancel)
            where TResponse : ITableauServerResponse
            where TExtractRefreshType : class, IExtractRefreshType
            where TExtractRefreshTask: IExtractRefreshTask<TSchedule>
        {
            var items = responseItemFactory(response).ExceptNulls().ToImmutableArray();
            var tasks = ImmutableArray.CreateBuilder<TExtractRefreshTask>(items.Length);

            foreach (var item in items)
            {
                var contentType = item.GetContentType();

                if(contentType is ExtractRefreshContentType.Unknown)
                {
                    logger.LogWarning(localizer[SharedResourceKeys.UnknownExtractRefreshContentTypeWarning], item.Id);
                    continue;
                }

                var finder = finderFactory.ForExtractRefreshContent(contentType);
                var contentReference = await finder.FindByIdAsync(item.GetContentId(), cancel).ConfigureAwait(false);

                /*
                 * Content reference is null when the referenced content item (e.g. workbook/data source)
                 * is in a private space or other "pre-manifest" filter.
                 * 
                 * We similarly filter out those extract refresh tasks.
                 */
                if(contentReference is not null)
                {
                    tasks.Add(await modelFactory(item, contentReference, cancel).ConfigureAwait(false));
                }
            }

            return tasks.ToImmutable();
        }
    }
}