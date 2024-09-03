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

namespace Tableau.Migration.Net
{
    /// <summary>
    /// Default <see cref="IRequestBuilder{TBuilder}"/> implementation.
    /// </summary>
    internal abstract class RequestBuilderFactory<TRequestBuilder> : IRequestBuilderFactory<TRequestBuilder>
        where TRequestBuilder : IRequestBuilder
    {
        private readonly Lazy<Uri> _baseUri;

        /// <summary>
        /// Gets the base URI for created URIs.
        /// </summary>
        protected Uri BaseUri => _baseUri.Value;

        public RequestBuilderFactory(IRequestBuilderFactoryInput input)
        {
            // Get the base URI lazily so input initialization can happen as late as possible.
            _baseUri = new(() => input.ServerUri ?? throw new InvalidOperationException($"{nameof(IRequestBuilderFactoryInput)} has not been initialized."));
        }

        /// <inheritdoc/>
        public abstract TRequestBuilder CreateUri(string path, bool useExperimental = false);

        /// <inheritdoc/>
        IRequestBuilder IRequestBuilderFactory.CreateUri(string path, bool useExperimental) => CreateUri(path, useExperimental);
    }
}
