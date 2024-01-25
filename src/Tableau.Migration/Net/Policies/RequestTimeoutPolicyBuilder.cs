﻿using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using Polly;
using Polly.Timeout;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Config;

namespace Tableau.Migration.Net.Policies
{
    internal class RequestTimeoutPolicyBuilder
        : IHttpPolicyBuilder
    {
        private static readonly HashSet<(HttpMethod, Regex)> _fileTransferRequests = new()
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
        private readonly IConfigReader _configReader;

        public RequestTimeoutPolicyBuilder(
            IConfigReader configReader)
        {
            _configReader = configReader;
        }

        public IAsyncPolicy<HttpResponseMessage>? Build(
            HttpRequestMessage httpRequest)
        {
            var sdkOptions = _configReader.Get();

            if (IsFileTransferRequest(httpRequest))
            {
                return Policy.TimeoutAsync<HttpResponseMessage>(
                    sdkOptions.Network.Resilience.PerFileTransferRequestTimeout,
                    TimeoutStrategy.Optimistic);
            }

            // Basic Timeout Per-Request Implementation
            // TODO: W-12611689 - Network Client - Timeout
            return Policy.TimeoutAsync<HttpResponseMessage>(
                sdkOptions.Network.Resilience.PerRequestTimeout,
                TimeoutStrategy.Optimistic);
        }

        public static bool IsFileTransferRequest(HttpRequestMessage httpRequest)
        => httpRequest.RequestUri is not null &&
            _fileTransferRequests.Any(lrr =>
            httpRequest.Method == lrr.Item1 &&
            lrr.Item2.IsMatch(httpRequest.RequestUri!.AbsolutePath));
    }
}
