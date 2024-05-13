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
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Api.Simulation.Rest.Net.Requests;
using Tableau.Migration.Net;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Responses
{
    internal class RestPermissionsGetResponseBuilder<TContent> : RestApiResponseBuilderBase<PermissionsResponse>
        where TContent : IRestIdentifiable, INamedContent
    {
        private readonly string _contentTypeUrlPrefix;
        private readonly Func<TableauData, ICollection<TContent>> _getContent;

        public RestPermissionsGetResponseBuilder(
            TableauData data,
            IHttpContentSerializer serializer,
            string contentTypeUrlPrefix,
            Func<TableauData, ICollection<TContent>> getContent)
            : base(data, serializer, requiresAuthentication: true)
        {
            _contentTypeUrlPrefix = contentTypeUrlPrefix.ToLower();
            _getContent = getContent;
        }

        protected override ValueTask<(PermissionsResponse Response, HttpStatusCode ResponseCode)> BuildResponseAsync(
            HttpRequestMessage request, CancellationToken cancel)
        {
            var contentId = request.GetIdAfterSegment(_contentTypeUrlPrefix);

            if (contentId is null)
                return BuildEmptyErrorResponseAsync(HttpStatusCode.BadRequest, 0, "URL content item's ID cannot be null.", "");

            var content = _getContent(Data).SingleOrDefault(d => d.Id == contentId);

            if (content is null)
                return BuildEmptyErrorResponseAsync(HttpStatusCode.NotFound, 0, $"The content item for ID {contentId} could not be found.", "");

            var contentTypePermissions = Data.GetContentTypePermissions(_contentTypeUrlPrefix);

            contentTypePermissions.TryGetValue(contentId.Value, out var permissions);

            return ValueTask.FromResult((new PermissionsResponse
            {
                Parent = new ParentContentType
                {
                    Id = contentId.Value
                },
                Item = permissions ?? new PermissionsType
                {
                    ContentItem = new PermissionsContentItemType
                    {
                        Id = content.Id,
                        Name = content.Name
                    }
                }
            },
            HttpStatusCode.OK));
        }
    }
}
