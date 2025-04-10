﻿//
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

using Microsoft.Extensions.Logging;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Endpoints.ContentClients
{
    /// <summary>
    /// Base class for content clients.
    /// </summary>
    /// <typeparam name="TContent"></typeparam>
    public abstract class ContentClientBase<TContent> : IContentClient<TContent>
    {
        /// <summary>
        /// The logger for the content client.
        /// </summary>
        protected ILogger<IContentClient<TContent>> Logger { get; }

        /// <summary>
        /// The localizer for the content client.
        /// </summary>
        protected ISharedResourcesLocalizer Localizer { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentClientBase{TContent}"/> class.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="localizer"></param>
        public ContentClientBase(
            ILogger<IContentClient<TContent>> logger,
            ISharedResourcesLocalizer localizer)
        {
            Logger = logger;
            Localizer = localizer;
        }
    }
}
