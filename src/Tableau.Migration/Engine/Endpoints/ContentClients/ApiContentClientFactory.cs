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
using Microsoft.Extensions.Logging;
using Tableau.Migration.Api;
using Tableau.Migration.Content;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Endpoints.ContentClients
{
    internal class ApiContentClientFactory : IContentClientFactory
    {
        private readonly ConcurrentDictionary<Type, object> _contentClients = new();
        private readonly ILoggerFactory _loggerFactory;
        private readonly ISharedResourcesLocalizer _localizer;
        private readonly ISitesApiClient _sitesApiClient;

        public ApiContentClientFactory(
            ISitesApiClient sitesApiClient,
            ILoggerFactory loggerFactory,
            ISharedResourcesLocalizer localizer)
        {
            _sitesApiClient = sitesApiClient;
            _loggerFactory = loggerFactory;
            _localizer = localizer;
        }

        /// <summary>
        /// Get a content client for a specific content type.
        /// </summary>
        /// <typeparam name="TContent">The content type of the client to get.</typeparam>
        /// <returns>The content client of the requested type.</returns>
        /// <exception cref="InvalidOperationException">Exception that is thrown if a content client of the requested type can not be created, usually because it does not exist.</exception>
        public IContentClient<TContent> GetContentClient<TContent>()
        {
            return (IContentClient<TContent>)_contentClients.GetOrAdd(
                typeof(TContent), _ => CreateContentClient<TContent>()
            );
        }

        /// <summary>
        /// Create a content client for the specified content type.
        /// </summary>
        /// <typeparam name="TContent">The content type of the client to create.</typeparam>
        /// <returns>The content client of the requested type.</returns>
        /// <exception cref="InvalidOperationException">Exception that is thrown if a content client of the requested type can not be created, usually because it does not exist.</exception>
        private IContentClient<TContent> CreateContentClient<TContent>()
        {
            if (typeof(TContent) == typeof(IWorkbook))
            {
                var logger = _loggerFactory.CreateLogger<IWorkbooksContentClient>();
                return (IContentClient<TContent>)new WorkbooksContentClient(_sitesApiClient.Workbooks, logger, _localizer);
            }

            if (typeof(TContent) == typeof(IView))
            {
                var logger = _loggerFactory.CreateLogger<IViewsContentClient>();
                return (IContentClient<TContent>)new ViewsContentClient(_sitesApiClient.Views, logger, _localizer);
            }

            // Add other content client types here as needed

            throw new InvalidOperationException($"Content client for type {typeof(TContent).Name} does not exist.");
        }
    }

}