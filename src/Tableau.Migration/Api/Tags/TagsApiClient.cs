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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models.Requests;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content;
using Tableau.Migration.Net;
using Tableau.Migration.Net.Rest;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Api.Tags
{
    internal sealed class TagsApiClient : ApiClientBase, ITagsApiClient
    {
        private readonly TagsUriBuilder _tagsUriBuilder;
        private readonly IHttpContentSerializer _serializer;

        public TagsApiClient(IRestRequestBuilderFactory restRequestBuilderFactory,
            ILoggerFactory loggerFactory,
            ISharedResourcesLocalizer sharedResourcesLocalizer,
            TagsUriBuilder tagsUriBuilder,
            IHttpContentSerializer serializer)
            : base(restRequestBuilderFactory,
                  loggerFactory,
                  sharedResourcesLocalizer)
        {
            _tagsUriBuilder = tagsUriBuilder;
            _serializer = serializer;
        }

        #region - ITagsApiClient<IDataSource> Implementation -

        /// <inheritdoc />
        public async Task<IResult<IImmutableList<ITag>>> AddTagsAsync(Guid contentItemId, IEnumerable<ITag> tags, CancellationToken cancel)
        {
            if (!tags.Any())
            {
                return Result<IImmutableList<ITag>>.Succeeded(ImmutableArray<ITag>.Empty);
            }

            var request = RestRequestBuilderFactory
                .CreateUri(_tagsUriBuilder.BuildUri(contentItemId))
                .ForPutRequest()
                .WithXmlContent(new AddTagsRequest(tags));

            var result = await request.SendAsync<AddTagsResponse>(cancel)
                            .ToResultAsync<AddTagsResponse, IImmutableList<ITag>>((response) =>
                                response.Items
                                .Select(item => (ITag)new Tag(item))
                                .ToImmutableArray(),
                                SharedResourcesLocalizer)
                            .ConfigureAwait(false);

            return result;
        }

        /// <inheritdoc />
        public async Task<IResult> RemoveTagsAsync(Guid contentItemId, IEnumerable<ITag> tags, CancellationToken cancel)
        {
            if (!tags.Any())
            {
                return Result.Succeeded();
            }

            var resultBuilder = new ResultBuilder();

            foreach (var tag in tags)
            {
                var deleteResult = await RemoveTagAsync(contentItemId, tag, cancel).ConfigureAwait(false);

                resultBuilder.Add(deleteResult);
            }

            return resultBuilder
                .Build();
        }

        private async Task<IResult> RemoveTagAsync(Guid contentItemId, ITag tag, CancellationToken cancel)
        {
            var request = RestRequestBuilderFactory
                .CreateUri($"{_tagsUriBuilder.BuildUri(contentItemId)}/{Uri.EscapeDataString(tag.Label)}")
                .ForDeleteRequest();

            return await request.SendAsync(cancel)
                .ToResultAsync(_serializer, SharedResourcesLocalizer, cancel)
                .ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IResult> UpdateTagsAsync(Guid contentItemId, IEnumerable<ITag> tags, CancellationToken cancel)
        {
            //add tags returns all existing tags, so we'll know what to remove. 
            var destinationTagsResult = await AddTagsAsync(contentItemId, tags, cancel)
                 .ConfigureAwait(false);

            if (!destinationTagsResult.Success)
            {
                return destinationTagsResult;
            }

            var expectedTags = tags.ToImmutableHashSet(TagLabelComparer.Instance);
            var tagsToRemove = destinationTagsResult.Value
                .Where(t => !expectedTags.Contains(t));

            return await RemoveTagsAsync(contentItemId, tagsToRemove, cancel)
                .ConfigureAwait(false);
        }

        #endregion
    }
}
