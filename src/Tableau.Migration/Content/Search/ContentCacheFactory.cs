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
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace Tableau.Migration.Content.Search
{
    internal class ContentCacheFactory : IContentCacheFactory
    {
        private readonly IServiceProvider _services;

        public ContentCacheFactory(IServiceProvider services)
        {
            _services = services;
        }

        public IContentCache<TContent>? ForContentType<TContent>([DoesNotReturnIf(true)] bool throwIfNotAvailable)
            where TContent : class, IContentReference
        {
            var cache = _services.GetService<IContentCache<TContent>>();

            if (cache is null && throwIfNotAvailable)
            {
                throw new ArgumentException($"No content cache was found for content type {typeof(TContent).Name}.", nameof(TContent));
            }

            return cache;
        }
    }
}
