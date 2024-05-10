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
using System.Collections.Immutable;
using System.Net.Http;
using System.Xml.Serialization;
using Tableau.Migration.Api.Rest.Models.Requests;
using Tableau.Migration.Net.Rest.Filtering;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Requests
{
    internal static class HttpRequestMessageExtensions
    {
        public static string? GetSegment(this HttpRequestMessage request, int index) => request.RequestUri.GetSegment(index);

        public static string? GetLastSegment(this HttpRequestMessage request) => request.RequestUri.GetLastSegment();

        public static string? GetNextToLastSegment(this HttpRequestMessage request) => request.RequestUri.GetNextToLastSegment();

        public static Guid GetRequestIdFromUri(this HttpRequestMessage request, bool hasSuffix = false) => request.RequestUri.GetRequestIdFromUri(hasSuffix);

        public static Guid? GetSiteIdFromUrl(this HttpRequestMessage request) => request.RequestUri.GetSiteIdFromUrl();

        public static Guid? GetProjectIdFromUrl(this HttpRequestMessage request) => request.RequestUri.GetProjectIdFromUrl();

        public static Guid? GetIdAfterSegment(this HttpRequestMessage request, string segmentBeforeId) => request.RequestUri.GetIdAfterSegment(segmentBeforeId);

        public static bool IsDefaultPermissionsRequest(this HttpRequestMessage request) => request.RequestUri.IsDefaultPermissionsRequest();

        public static string ParseDefaultPermissionsContentType(this HttpRequestMessage request) => request.RequestUri.ParseDefaultPermissionsContentType();

        public static IImmutableList<Filter> ParseFilters(this HttpRequestMessage request) => request.RequestUri.ParseFilters();

        public static string GetQueryStringValue(this HttpRequestMessage request, string key) => request.RequestUri.GetQueryStringValue(key);

        public static TRequest? GetTableauServerRequest<TRequest>(this HttpRequestMessage request)
            where TRequest : TableauServerRequest
        {
            var xmlContent = request.Content?.ReadAsStreamAsync().Result;

            if (xmlContent == null)
                return null;

            var xmlSerializer = new XmlSerializer(typeof(TRequest));

            return (TRequest?)xmlSerializer.Deserialize(xmlContent);
        }
    }
}
