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
    /// Abstract base class for <see cref="IContainerContent"/> content items.
    /// </summary>
    public abstract class ContainerContentBase : MappableContainerContentBase, IContainerContent
    {
        /// <summary>
        /// Creates a new <see cref="ContainerContentBase"/> object.
        /// </summary>
        /// <param name="container">The content container.</param>
        protected ContainerContentBase(IContentReference container)
        {
            Container = container;
        }

        /// <inheritdoc/>
        virtual public IContentReference Container { get; protected set; }

        /// <inheritdoc/>
        protected override IContentReference? MappableContainer
        {
            get => Container;
            set => Container = Guard.AgainstNull(value, () => value);
        }
    }
}
