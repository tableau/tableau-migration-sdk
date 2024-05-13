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
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Requests;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Simulation.Rest.Net;
using Tableau.Migration.Api.Simulation.Rest.Net.Requests;
using Tableau.Migration.Api.Simulation.Rest.Net.Responses;
using Tableau.Migration.Net.Simulation;

using static Tableau.Migration.Api.Simulation.Rest.Net.Requests.RestUrlPatterns;

namespace Tableau.Migration.Api.Simulation.Rest.Api
{
    /// <summary>
    /// Base class for objects that define simulation of Tableau REST API permissions and tags methods.
    /// </summary>
    public abstract class TagsRestApiSimulatorBase<TContent, TTag>
        : PermissionsRestApiSimulatorBase<TContent>
        where TContent : IRestIdentifiable, INamedContent, IWithTagTypes
        where TTag : ITagType, new()
    {
        /// <summary>
        /// Gets the simulated add tags API method.
        /// </summary>
        public MethodSimulator AddTags { get; }

        /// <summary>
        /// Gets the simulated remove tags API method.
        /// </summary>
        public MethodSimulator RemoveTags { get; }

        /// <summary>
        /// Creates a new <see cref="TagsRestApiSimulatorBase{TContent, TTag}"/> object.
        /// </summary>
        /// <param name="simulator">A response simulator to setup with REST API methods.</param>
        /// <param name="contentTypeUrlPrefix">The content type's URl prefix.</param>
        /// <param name="getContent">Delegate used to retrieve content items by ID.</param>
        public TagsRestApiSimulatorBase(
            TableauApiResponseSimulator simulator,
            string contentTypeUrlPrefix,
            Func<TableauData, ICollection<TContent>> getContent)
            : base(simulator, contentTypeUrlPrefix, getContent)
        {
            AddTags = simulator.SetupRestPutList<AddTagsResponse, AddTagsResponse.TagType>(
                SiteEntityUrl(ContentTypeUrlPrefix, "tags"),
                BuildAddTagsDelegate(getContent));

            RemoveTags = simulator.SetupRestDelete(
                SiteEntityUrl(ContentTypeUrlPrefix, $"tags/{new Regex(NamePattern, RegexOptions.IgnoreCase)}"),
                new RestDeleteResponseBuilder(simulator.Data,
                BuildDeleteTagsDelegate(getContent),
                simulator.Serializer));
        }

        #region - Response Delegates -

        private Func<TableauData, HttpRequestMessage, HttpStatusCode> BuildDeleteTagsDelegate(Func<TableauData, IEnumerable<TContent>> getContent)
        {
            return (data, request) =>
            {
                var contentId = request.GetIdAfterSegment(ContentTypeUrlPrefix);
                var tagLabel = request.GetLastSegment();
                if (contentId == null || string.IsNullOrEmpty(tagLabel))
                {
                    return HttpStatusCode.BadRequest;
                }

                var content = getContent(data).FirstOrDefault(ds => ds.Id == contentId.Value);

                var existingTags = content?.Tags.ToList();

                if (content == null || existingTags == null)
                {
                    return HttpStatusCode.BadRequest;
                }

                content.Tags = existingTags
                    .Where(existingTag => existingTag.Label != tagLabel)
                    .Select(existingTag => (ITagType)new TTag()
                    {
                        Label = existingTag.Label
                    })
                    .ToArray();

                return HttpStatusCode.OK;
            };
        }

        private Func<TableauData, HttpRequestMessage, ICollection<AddTagsResponse.TagType>> BuildAddTagsDelegate(Func<TableauData, IEnumerable<TContent>> getContent)
        {
            return (data, request) =>
            {
                var contentId = request.GetIdAfterSegment(ContentTypeUrlPrefix);

                if (contentId is null)
                {
                    return new List<AddTagsResponse.TagType>();
                }
                var content = getContent(data).FirstOrDefault(ds => ds.Id == contentId.Value);

                if (content is null)
                {
                    return new List<AddTagsResponse.TagType>();
                }

                var tags = content.Tags.ToList();

                var requestContent = request.GetTableauServerRequest<AddTagsRequest>();

                if (requestContent is null)
                {
                    return tags.Select(tag => new AddTagsResponse.TagType(tag)).ToArray();
                }

                var newTags = requestContent.Tags;
                if (newTags is null)
                {
                    return tags.Select(tag => new AddTagsResponse.TagType(tag)).ToArray();
                }

                foreach (var newTag in newTags)
                {
                    if (tags.Any(e => e.Label == newTag.Label))
                    {
                        continue;
                    }

                    tags.Add(new TTag()
                    {
                        Label = newTag.Label
                    });
                }

                content.Tags = tags.ToArray();

                return tags.Select(tag => new AddTagsResponse.TagType(tag)).ToArray();
            };
        }

        #endregion
    }
}
