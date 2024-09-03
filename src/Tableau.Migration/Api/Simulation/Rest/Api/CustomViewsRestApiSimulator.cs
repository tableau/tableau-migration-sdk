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

using System.Collections.Generic;
using System.Linq;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Simulation.Rest.Net;
using Tableau.Migration.Api.Simulation.Rest.Net.Requests;
using Tableau.Migration.Api.Simulation.Rest.Net.Responses;
using Tableau.Migration.Net.Simulation;
using static Tableau.Migration.Api.Simulation.Rest.Net.Requests.RestUrlPatterns;

namespace Tableau.Migration.Api.Simulation.Rest.Api
{
    /// <summary>
    /// Object that defines simulation of Tableau REST API custom view methods.
    /// </summary>
    public sealed class CustomViewsRestApiSimulator
    {
        /// <summary>
        /// Gets the simulated custom view query API method.
        /// </summary>
        public MethodSimulator QueryCustomViews { get; }

        /// <summary>
        /// Gets the simulated custom view download API method.
        /// </summary>
        public MethodSimulator DownloadCustomView { get; }

        /// <summary>
        /// Gets the simulated commit custom view upload API method.
        /// </summary>
        public MethodSimulator CommitCustomViewUpload { get; }


        /// <summary>
        /// Gets the simulated get custom view default users API method.
        /// </summary>
        public MethodSimulator GetCustomViewDefaultUsers { get; }


        /// <summary>
        /// Sets the simulated set custom view default users API method.
        /// </summary>
        public MethodSimulator SetCustomViewDefaultUsers { get; }

        /// <summary>
        /// Creates a new <see cref="CustomViewsRestApiSimulator"/> object.
        /// </summary>
        /// <param name="simulator">A response simulator to setup with REST API methods.</param>
        public CustomViewsRestApiSimulator(TableauApiResponseSimulator simulator)
        {
            QueryCustomViews = simulator.SetupRestPagedList<CustomViewsResponse, CustomViewsResponse.CustomViewResponseType>(
                SiteUrl(RestUrlPrefixes.CustomViews),
                (data, request) =>
                {
                    return data.CustomViews.Select(x => new CustomViewsResponse.CustomViewResponseType(x)).ToList();
                });

            DownloadCustomView = simulator.SetupRestDownloadById(
              SiteEntityUrl(
                  postSitePreEntitySuffix: RestUrlPrefixes.CustomViews,
                  postEntitySuffix: "content",
                  useExperimental: true),
              (data) => data.CustomViewFiles, 4);

            CommitCustomViewUpload = simulator.SetupRestPost(
                SiteUrl(RestUrlPrefixes.CustomViews, useExperimental: true),
                new RestCommitCustomViewUploadResponseBuilder(simulator.Data, simulator.Serializer));

            GetCustomViewDefaultUsers = simulator.SetupRestPagedList<UsersWithCustomViewAsDefaultViewResponse, UsersWithCustomViewAsDefaultViewResponse.UserType>(
                SiteEntityUrl(RestUrlPrefixes.CustomViews, "/default/users"),
                 (data, request) =>
                 {
                     var customViewId = request.GetIdAfterSegment(RestUrlPrefixes.CustomViews);

                     if (data.CustomViewDefaultUsers == null || customViewId == null)
                         return [];

                     data.CustomViewDefaultUsers.TryGetValue(customViewId.Value, out List<UsersWithCustomViewAsDefaultViewResponse.UserType>? defaultUsers);

                     return defaultUsers == null ? [] : defaultUsers;
                 });


            SetCustomViewDefaultUsers = simulator.SetupRestPost(
                 SiteEntityUrl(RestUrlPrefixes.CustomViews, "/default/users"),
                 new RestCustomViewDefaultUsersAddResponseBuilder(simulator.Data, simulator.Serializer));
        }
    }
}
