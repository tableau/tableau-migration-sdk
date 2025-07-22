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
using Tableau.Migration.Api.Rest.Models;

namespace Tableau.Migration.Content.Permissions
{
    /// <summary>
    /// Interface for the permission information of a content item.
    /// </summary>
    public interface IPermissions : IPermissionSet
    {
        /// <summary>
        /// The ID of the parent content item that is determining permissions, such as a locked project.
        /// The parent content can be one of the types in <see cref="ParentContentTypeNames"/>,
        /// and will be null if the permissions are determined by the content item directly.
        /// </summary>
        public Guid? ParentId { get; }
    }
}
