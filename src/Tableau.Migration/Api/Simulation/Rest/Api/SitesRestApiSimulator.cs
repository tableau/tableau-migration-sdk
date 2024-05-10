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
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using Tableau.Migration.Api.Rest.Models.Requests;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Simulation.Rest.Net;
using Tableau.Migration.Api.Simulation.Rest.Net.Requests;
using Tableau.Migration.Net.Simulation;

using static Tableau.Migration.Api.Simulation.Rest.Net.Requests.RestUrlPatterns;

namespace Tableau.Migration.Api.Simulation.Rest.Api
{
    /// <summary>
    /// Object that defines simulation of Tableau REST API site methods.
    /// </summary>
    public sealed class SitesRestApiSimulator
    {
        /// <summary>
        /// Gets the simulated site query by ID API method.
        /// </summary>
        public MethodSimulator QuerySiteById { get; }

        /// <summary>
        /// Gets the simulated site query by content URL API method.
        /// </summary>
        public MethodSimulator QuerySiteByContentUrl { get; }

        /// <summary>
        /// Gets the simulated update site API method.
        /// </summary>
        public MethodSimulator UpdateSite { get; }

        /// <summary>
        /// Creates a new <see cref="SitesRestApiSimulator"/> object.
        /// </summary>
        /// <param name="simulator">A response simulator to setup with REST API methods.</param>
        public SitesRestApiSimulator(TableauApiResponseSimulator simulator)
        {
            QuerySiteById = simulator.SetupRestGetById<SiteResponse, SiteResponse.SiteType>(RestApiUrl($"sites/{SiteId}"), d => d.Sites);
            QuerySiteByContentUrl = simulator.SetupRestGetByContentUrl<SiteResponse, SiteResponse.SiteType>(RestApiUrl($"sites/{ContentUrlPattern}"), d => d.Sites,
                queryStringPatterns: new[] { ("key", new Regex("contentUrl")) });

            UpdateSite = simulator.SetupRestPut<SiteResponse, SiteResponse.SiteType>(RestApiUrl($"sites/{SiteId}"), UpdateSiteFromRequest);
        }

        private static SiteResponse.SiteType? UpdateSiteFromRequest(TableauData data, HttpRequestMessage request)
        {
            var id = request.GetIdAfterSegment("sites");
            if (id is null)
            {
                throw new InvalidOperationException("Site ID should not be null");
            }

            var site = data.Sites.SingleOrDefault(s => s.Id == id.Value);
            if (site is null)
            {
                return null;
            }

            var updateRequest = request.GetTableauServerRequest<UpdateSiteRequest>()?.Site;
            if (updateRequest is null)
            {
                return null;
            }

            if (updateRequest.ExtractEncryptionMode is not null)
            {
                site.ExtractEncryptionMode = updateRequest.ExtractEncryptionMode;
            }

            return site;
        }
    }
}
