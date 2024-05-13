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
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Simulation.Rest.Net;
using Tableau.Migration.Api.Simulation.Rest.Net.Responses;
using Tableau.Migration.Net.Simulation;

using static Tableau.Migration.Api.Simulation.Rest.Net.Requests.RestUrlPatterns;

namespace Tableau.Migration.Api.Simulation.Rest.Api
{
    /// <summary>
    /// Base class for objects that define simulation of Tableau REST API permissions methods.
    /// </summary>
    public abstract class PermissionsRestApiSimulatorBase<TContent>
        where TContent : IRestIdentifiable, INamedContent
    {
        /// <summary>
        /// Gets the URL prefix for the content permissions.
        /// </summary>
        protected readonly string ContentTypeUrlPrefix;

        /// <summary>
        /// Gets the simulated permission create API method.
        /// </summary>
        public MethodSimulator CreatePermissions { get; }

        /// <summary>
        /// Gets the simulated permission query API method.
        /// </summary>
        public MethodSimulator QueryPermissions { get; }

        /// <summary>
        /// Creates a new <see cref="PermissionsRestApiSimulatorBase{TContent}"/> object.
        /// </summary>
        /// <param name="simulator">A response simulator to setup with REST API methods.</param>
        /// <param name="contentTypeUrlPrefix">The content type's URl prefix.</param>
        /// <param name="getContent">Delegate used to retrieve content items by ID.</param>
        public PermissionsRestApiSimulatorBase(
            TableauApiResponseSimulator simulator,
            string contentTypeUrlPrefix,
            Func<TableauData, ICollection<TContent>> getContent)
        {
            ContentTypeUrlPrefix = contentTypeUrlPrefix;

            CreatePermissions = simulator.SetupRestPut(
                SiteEntityUrl(ContentTypeUrlPrefix, "permissions"),
                new RestPermissionsCreateResponseBuilder<TContent>(simulator.Data, simulator.Serializer, ContentTypeUrlPrefix, getContent));

            QueryPermissions = simulator.SetupRestGet(
                SiteEntityUrl(ContentTypeUrlPrefix, "permissions"),
                new RestPermissionsGetResponseBuilder<TContent>(simulator.Data, simulator.Serializer, ContentTypeUrlPrefix, getContent));
        }
    }
}
