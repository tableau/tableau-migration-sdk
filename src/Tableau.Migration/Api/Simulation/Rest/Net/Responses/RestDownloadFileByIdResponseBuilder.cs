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
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Simulation.Rest.Net.Requests;
using Tableau.Migration.Net;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Responses
{
    internal class RestDownloadFileByIdResponseBuilder : EmptyRestResponseBuilder
    {
        private readonly Func<TableauData, IReadOnlyDictionary<Guid, byte[]>> _getFilesById;
        private readonly int _idNotFoundSubCode;

        public RestDownloadFileByIdResponseBuilder(
            TableauData data,
            IHttpContentSerializer serializer,
            Func<TableauData, IReadOnlyDictionary<Guid, byte[]>> getFilesById,
            int idNotFoundSubCode,
            bool requiresAuthentication)
            : base(data, serializer, requiresAuthentication)
        {
            _getFilesById = getFilesById;
            _idNotFoundSubCode = idNotFoundSubCode;
        }

        protected override Task<HttpResponseMessage> BuildResponseAsync(HttpRequestMessage request, CancellationToken cancel)
        {
            var filesById = _getFilesById(Data);

            var entityId = request.GetRequestIdFromUri(hasSuffix: true);
            if (!filesById.TryGetValue(entityId, out var file))
            {
                var errorBuilder = new StaticRestErrorBuilder(HttpStatusCode.NotFound, _idNotFoundSubCode, string.Empty, string.Empty);
                return Task.FromResult(BuildErrorResponse(request, errorBuilder));
            }

            return Task.FromResult(new HttpResponseMessage
            {
                Content = new ByteArrayContent(file)
            });
        }
    }
}