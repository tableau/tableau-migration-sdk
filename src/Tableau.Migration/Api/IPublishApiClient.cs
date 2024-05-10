﻿//
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

using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Api
{
    /// <summary>
    /// Interface for a content typed API client that can publish items.
    /// </summary>
    /// <typeparam name="TPublish">The content publish type.</typeparam>
    /// <typeparam name="TPublishResult">The publish result type.</typeparam>
    public interface IPublishApiClient<TPublish, TPublishResult>
        where TPublishResult : class, IContentReference
    {
        /// <summary>
        /// Publishes a content item.
        /// </summary>
        /// <param name="item">The content item to publish.</param>
        /// <param name="cancel">A cancellation token to obey.</param>
        /// <returns>The results of the publishing with a content reference of the newly published item.</returns>
        Task<IResult<TPublishResult>> PublishAsync(TPublish item, CancellationToken cancel);
    }

    /// <summary>
    /// Interface for a content typed API client that can publish items.
    /// </summary>
    /// <typeparam name="TPublish">The content publish type.</typeparam>
    public interface IPublishApiClient<TPublish> : IPublishApiClient<TPublish, TPublish>
        where TPublish : class, IContentReference
    { }
}
