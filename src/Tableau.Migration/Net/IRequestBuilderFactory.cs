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

namespace Tableau.Migration.Net
{
    /// <summary>
    /// Interface for <see cref="IRequestBuilder"/> factories.
    /// </summary>
    public interface IRequestBuilderFactory
    {
        /// <summary>
        /// Creates a new <see cref="IRequestBuilder"/> instance.
        /// </summary>
        /// <param name="path">The URI path.</param>
        /// <param name="useExperimental">Flag indicating if the experimental API Version should be used.</param>
        /// <returns>A new <see cref="IRequestBuilder"/> instance.</returns>
        IRequestBuilder CreateUri(string path, bool useExperimental = false);
    }

    /// <summary>
    /// Interface for <see cref="IRequestBuilder"/> factories.
    /// </summary>
    public interface IRequestBuilderFactory<TRequestBuilder> : IRequestBuilderFactory
        where TRequestBuilder : IRequestBuilder
    {
        /// <summary>
        /// Creates a new <typeparamref name="TRequestBuilder"/> instance.
        /// </summary>
        /// <param name="path">The URI path.</param>
        /// <param name="useExperimental">Flag indicating if the experimental API Version should be used.</param>
        /// <returns>A new <typeparamref name="TRequestBuilder"/> instance.</returns>
        new TRequestBuilder CreateUri(string path, bool useExperimental = false);
    }
}
