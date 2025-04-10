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

namespace Tableau.Migration.Api.EmbeddedCredentials
{
    internal interface IEmbeddedCredentialsApiClientFactory
    {
        /// <summary>
        /// Creates the <see cref="IEmbeddedCredentialsApiClient"/> from the given <see cref="IContentApiClient"/>.
        /// </summary>
        /// <param name="contentApiClient">The <see cref="IContentApiClient"/>, for example, <see cref="IWorkbooksApiClient"/>.</param>
        /// <returns>The <see cref="IEmbeddedCredentialsApiClient"/> for the input <see cref="IContentApiClient"/>.</returns>
        IEmbeddedCredentialsApiClient Create(IContentApiClient contentApiClient);
    }
}