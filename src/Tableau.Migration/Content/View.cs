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

using System.Collections.Generic;
using Tableau.Migration.Api.Rest.Models;

namespace Tableau.Migration.Content
{
    internal sealed class View : ContentBase, IView
    {
        /// <inheritdoc />
        public IList<ITag> Tags { get; set; }

        public View(IViewReferenceType view, IContentReference project, string? workbookName)
        {
            Guard.AgainstNullEmptyOrWhiteSpace(workbookName, () => workbookName);

            Id = view.Id;
            Name = Guard.AgainstNullEmptyOrWhiteSpace(view.Name, () => view.Name);
            ContentUrl = Guard.AgainstNull(view.ContentUrl, () => view.ContentUrl);
            Location = project.Location.Append(workbookName).Append(Name);
            Tags = view.Tags.ToTagList(t => new Tag(t));
        }
    }
}
