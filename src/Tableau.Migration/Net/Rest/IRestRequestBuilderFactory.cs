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

namespace Tableau.Migration.Net.Rest
{
    /// <summary>
    /// Interface for <see cref="IRestRequestBuilder"/> factories.
    /// </summary>
    public interface IRestRequestBuilderFactory : IRequestBuilderFactory<IRestRequestBuilder>
    {
        /// <summary>
        /// Sets the default API version to use when creating <see cref="IRestRequestBuilder"/> instances. 
        /// </summary>
        /// <param name="version">The API version.</param>
        void SetDefaultApiVersion(string? version);

        /// <summary>
        /// Sets the default site ID to use when creating <see cref="IRestRequestBuilder"/> instances. 
        /// </summary>
        /// <param name="siteId">The site ID.</param>
        void SetDefaultSiteId(Guid? siteId);

        /// <summary>
        /// Sets the default site ID to use when creating <see cref="IRestRequestBuilder"/> instances. 
        /// </summary>
        /// <param name="siteId">The site ID.</param>
        void SetDefaultSiteId(string? siteId);
    }
}