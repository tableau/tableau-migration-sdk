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
using Tableau.Migration.Api.Rest.Models;

namespace Tableau.Migration.Content
{
    internal class CustomView : ContentBase, ICustomView
    {
        /// <inheritdoc/>
        public string CreatedAt { get; }

        /// <inheritdoc/>
        public string UpdatedAt { get; }

        /// <inheritdoc/>
        public string? LastAccessedAt { get; }

        /// <inheritdoc/>
        public bool Shared { get; set; }

        /// <inheritdoc/>
        public Guid BaseViewId { get; }

        /// <inheritdoc/>
        public string BaseViewName { get; }

        /// <inheritdoc/>
        public IContentReference Workbook { get; set; }

        /// <inheritdoc/>
        public IContentReference Owner { get; set; }

        internal CustomView(
            Guid id,
            string? name,
            string? createdAt,
            string? updatedAt,
            string? lastAccessedAt,
            bool shared,
            Guid? viewId,
            string? viewName,
            IContentReference workbook,
            IContentReference owner)
             : base(new ContentReferenceStub(
                 Guard.AgainstDefaultValue(id, () => id),
                 string.Empty,
                 new(Guard.AgainstNullEmptyOrWhiteSpace(name, () => name))))
        {
            CreatedAt = createdAt ?? string.Empty;
            UpdatedAt = updatedAt ?? string.Empty;
            LastAccessedAt = lastAccessedAt;
            Shared = shared;

            Guard.AgainstNull(viewId, () => viewId);
            BaseViewId = Guard.AgainstDefaultValue(viewId.Value, () => viewId);

            BaseViewName = Guard.AgainstNullEmptyOrWhiteSpace(viewName, () => viewName);
            Workbook = workbook;
            Owner = owner;
        }

        public CustomView(
            ICustomViewType response,
            IContentReference workbook,
            IContentReference owner)
            : this(
                  response.Id,
                  response.Name,
                  response.CreatedAt,
                  response.UpdatedAt,
                  response.LastAccessedAt,
                  response.Shared,
                  response.ViewId,
                  response.ViewName,
                  workbook,
                  owner)
        { }

        public CustomView(ICustomView customView)
            : this(
                 customView.Id,
                 customView.Name,
                 customView.CreatedAt,
                 customView.UpdatedAt,
                 customView.LastAccessedAt,
                 customView.Shared,
                 customView.BaseViewId,
                 customView.BaseViewName,
                 customView.Workbook,
                 customView.Owner)
        { }

    }
}
