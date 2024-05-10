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

using Microsoft.Extensions.Logging;
using Tableau.Migration.Api.Permissions;
using Tableau.Migration.Api.Tags;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Net.Rest;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Api
{
    internal sealed class ViewsApiClient : ContentApiClientBase, IViewsApiClient
    {
        public ViewsApiClient(
            IRestRequestBuilderFactory restRequestBuilderFactory,
            IPermissionsApiClientFactory permissionsClientFactory,
            IContentReferenceFinderFactory finderFactory,
            ILoggerFactory loggerFactory,
            ISharedResourcesLocalizer sharedResourcesLocalizer,
            ITagsApiClientFactory tagsClientFactory)
            : base(restRequestBuilderFactory, finderFactory, loggerFactory, sharedResourcesLocalizer)
        {
            Permissions = permissionsClientFactory.Create(this);
            Tags = tagsClientFactory.Create(this);
        }

        #region - IPermissionsContentApiClientImplementation -

        /// <inheritdoc />
        public IPermissionsApiClient Permissions { get; }

        #endregion

        #region - ITagsContentApiClient Implementation -

        /// <inheritdoc />
        public ITagsApiClient Tags { get; }

        #endregion
    }
}