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

namespace Tableau.Migration.Content
{
    /// <summary>
    /// Abstract base class for default <see cref="IMappableContainerContent"/> implementation.
    /// </summary>
    public abstract class MappableContainerContentBase : ContentBase, IMappableContainerContent
    {
        //We use a protected property and an explicit interface implementation
        //So that implementation types can use a more natural name (e.g. Project.ParentProject).

        /// <summary>
        /// Gets or sets the current mappable project/container reference.
        /// </summary>
        protected abstract IContentReference? MappableContainer { get; set; }

        /// <inheritdoc/>
        IContentReference? IMappableContainerContent.Container => MappableContainer;

        /// <inheritdoc/>
        void IMappableContainerContent.SetLocation(IContentReference? container, ContentLocation newLocation)
        {
            MappableContainer = container;
            Location = newLocation;
            Name = newLocation.Name;
        }
    }
}
