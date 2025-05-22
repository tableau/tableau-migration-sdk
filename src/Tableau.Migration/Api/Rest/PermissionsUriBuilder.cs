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

using System;
using Tableau.Migration.Content.Permissions;
using Tableau.Migration.Net.Rest;

namespace Tableau.Migration.Api.Rest
{
    internal class PermissionsUriBuilder : ContentItemUriBuilderBase, IPermissionsUriBuilder
    {
        public PermissionsUriBuilder(string prefix, string suffix = RestUrlKeywords.Permissions)
            : base(prefix, suffix)
        { }

        public virtual string BuildDeleteUri(
            Guid contentItemId,
            ICapability capability,
            GranteeType granteeType,
            Guid granteeId)
        {
            Guard.AgainstDefaultValue(contentItemId, nameof(contentItemId));
            Guard.AgainstDefaultValue(granteeId, nameof(granteeId));
            Guard.AgainstNullEmptyOrWhiteSpace(capability.Name, nameof(capability.Name));
            Guard.AgainstNullEmptyOrWhiteSpace(capability.Mode, nameof(capability.Mode));

            return $"{BuildUri(contentItemId)}/{granteeType.ToUrlSegment()}/{granteeId.ToUrlSegment()}/{capability.Name}/{capability.Mode}";
        }
    }
}
