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
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Net;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Responses
{
    internal class RestInitiateFileUploadResponseBuilder : RestApiResponseBuilderBase<FileUploadResponse>
    {
        public RestInitiateFileUploadResponseBuilder(
            TableauData data,
            IHttpContentSerializer serializer)
            : base(data, serializer, true)
        { }

        protected override ValueTask<(FileUploadResponse Response, HttpStatusCode ResponseCode)> BuildResponseAsync(
            HttpRequestMessage request, CancellationToken cancel)
        {
            var sessionId = Guid.NewGuid().ToString();
            var fileSize = 0;

            // Initiate Tableau Data with a 0 file size to indicate the upload request was initiated.
            Data.UpdateFile(sessionId, Array.Empty<byte>());

            return ValueTask.FromResult((new FileUploadResponse
            {
                Item = new FileUploadResponse.FileUploadType
                {
                    UploadSessionId = sessionId,
                    FileSize = fileSize,
                }
            },
            HttpStatusCode.Created));
        }
    }
}
