// Copyright (c) 2023, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the ""License"") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an ""AS IS"" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Tableau.Migration.Api.Rest.Models;

namespace Tableau.Migration.Content
{
    internal abstract class ViewsWorkbook : Workbook, IChildPermissionsContent
    {
        /// <inheritdoc />
        public IImmutableList<IView> Views { get; }

        public ViewsWorkbook(IWorkbookType response, IContentReference project, IContentReference owner, IImmutableList<IView> views)
            : base(response, project, owner)
        {
            Views = views;
        }

        #region - IChildPermissionsContent Implementation -

        Type IChildPermissionsContent.ChildType { get; } = typeof(IView);

        IEnumerable<IContentReference> IChildPermissionsContent.ChildPermissionContentItems => Views;

        bool IChildPermissionsContent.ShouldMigrateChildPermissions => !ShowTabs;

        #endregion
    }
}
