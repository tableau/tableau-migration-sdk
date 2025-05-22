//
//  Copyright (c) 2025, Salesforce, Inc.
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
    /// Base class for objects that define simulation of Tableau REST API permissions, tags, and embedded credentials methods.
    /// </summary>
    public abstract class EmbeddedCredentialsRestApiSimulatorBase<TContent, TTag>
        : TagsRestApiSimulatorBase<TContent, TTag>
        where TContent : IRestIdentifiable, INamedContent, IWithTagTypes
        where TTag : ITagType, new()
    {
        /// <summary>
        /// Gets the simulated apply keychain API method.
        /// </summary>
        public MethodSimulator ApplyKeychain { get; }

        /// <summary>
        /// Gets the simulated retrieve keychain API method.
        /// </summary>
        public MethodSimulator RetrieveKeychain { get; }

        /// <summary>
        /// Creates a new <see cref="EmbeddedCredentialsRestApiSimulatorBase{TContent, TTag}"/> object.
        /// </summary>
        /// <param name="simulator">A response simulator to setup with REST API methods.</param>
        /// <param name="contentTypeUrlPrefix">The content type's URl prefix.</param>
        /// <param name="getContent">Delegate used to retrieve content items by ID.</param>
        /// <param name="getKeychains">Delegate used to retrieve keychains of content items by ID.</param>
        protected EmbeddedCredentialsRestApiSimulatorBase(TableauApiResponseSimulator simulator, string contentTypeUrlPrefix,
            Func<TableauData, ICollection<TContent>> getContent,
            Func<TableauData, ConcurrentDictionary<Guid, RetrieveKeychainResponse>> getKeychains)
            : base(simulator, contentTypeUrlPrefix, getContent)
        {
            ApplyKeychain = simulator.SetupRestPut(SiteEntityUrl(ContentTypeUrlPrefix, RestUrlKeywords.ApplyKeychain),
                new EmptyRestResponseBuilder(simulator.Data, simulator.Serializer,
                (data, request) =>
                {
                    var id = request.GetRequestIdFromUri(hasSuffix: true);

                    var applyKeychain = request.GetTableauServerRequest<ApplyKeychainRequest>();
                    if (applyKeychain is not null)
                    {
                        var destinationUserIds = applyKeychain.AssociatedUserLuidMapping?.Select(m => m.DestinationSiteUserLuid);
                        var keychains = new RetrieveKeychainResponse(applyKeychain.EncryptedKeychains, destinationUserIds);

                        getKeychains(data).AddOrUpdate(id, keychains, (k, _) => keychains);
                    }
                },
                requiresAuthentication: true));

            RetrieveKeychain = simulator.SetupRestPost(SiteEntityUrl(ContentTypeUrlPrefix, RestUrlKeywords.RetrieveKeychain),
                (data, request) =>
                {
                    var id = request.GetRequestIdFromUri(hasSuffix: true);
                    if (!getKeychains(data).TryGetValue(id, out var response))
                    {
                        response = new();
                    }

                    return response;
                });
        }
    }
}
