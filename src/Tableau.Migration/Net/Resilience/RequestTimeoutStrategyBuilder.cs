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
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Polly;
using Polly.Timeout;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Config;

namespace Tableau.Migration.Net.Resilience
{
    internal class RequestTimeoutStrategyBuilder
        : IResilienceStrategyBuilder
    {
        private static readonly HashSet<(HttpMethod Method, Regex Pattern)> _fileTransferRequests = new()
        {
            // Regex to capture the download content AbsolutePath:
            // - GET /api/api-version/sites/site-id/datasources/datasource-id/content?includeExtract=extract-value
            //      - Link: https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_data_sources.htm#download_data_source
            // - GET /api/api-version/sites/site-id/workbooks/workbook-id/content?includeExtract=extract-value:
            //      - Link: https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_workbooks_and_views.htm#download_workbook
            //
            // Single-line Regex: ^.+(\/(datasources|workbooks)\/.+\/content){1}$
            // - "^" and "$" chars: Archors start and end.
            // - First ".+": Start with any char. Matches "/api/api-version/sites/site-id"
            // - "\/": Escaped Slash
            // - Group "(datasources|workbooks)": Only for datasources or workbooks
            // - Second ".*": Matches "datasource-id" or "workbook-id".
            // - "{1}": Exactly one match. 
            (HttpMethod.Get, new Regex($@"^.+(\/({RestUrlPrefixes.DataSources}|{RestUrlPrefixes.Workbooks})\/.+\/{RestUrlPrefixes.Content}){{1}}$", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase)),
            
            // Regex to capture the file upload content AbsolutePath:
            // - PUT /api/api-version/sites/site-id/fileUploads/upload-session-id
            //      - Link: https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_publishing.htm#append_to_file_upload
            //
            // Single-line Regex for ^.+(\/(fileUploads)\/.+){1}$
            // - "^" and "$" chars: Archors.
            // - First ".+": Start with any char. Matching the site request and the site GUID
            // - "\/": Escaped Slash
            // - Second ".*": Matching the  session GUID.
            // - "{1}": Exactly one match. 
            (HttpMethod.Put, new Regex($@"^.+(\/({RestUrlPrefixes.FileUploads})\/.+){{1}}$", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase)),
        };

        private static bool IsFileTransferRequest(HttpRequestMessage httpRequest)
        => httpRequest.RequestUri is not null &&
           _fileTransferRequests.Any(lrr => httpRequest.Method == lrr.Method && lrr.Pattern.IsMatch(httpRequest.RequestUri!.AbsolutePath));

        private static TimeSpan GetRequestTimeout(TimeoutGeneratorArguments args, MigrationSdkOptions options)
        {
            if (IsFileTransferRequest(args.Context.GetRequest()))
            {
                return options.Network.Resilience.PerFileTransferRequestTimeout;
            }

            return options.Network.Resilience.PerRequestTimeout;
        }

        /// <inheritdoc />
        public void Build(ResiliencePipelineBuilder<HttpResponseMessage> pipelineBuilder, MigrationSdkOptions options, ref Action? onPipelineDisposed)
        {
            pipelineBuilder.AddTimeout(new TimeoutStrategyOptions()
            {
                TimeoutGenerator = args => ValueTask.FromResult(GetRequestTimeout(args, options))
            });
        }
    }
}
