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

namespace Tableau.Migration.Engine.Hooks.Filters
{
    /// <summary>
    /// Interface for an object that can filter content of a specific content type, to determine which content to migrate.
    /// </summary>
    /// <typeparam name="TContent">Type of entity to be filtered</typeparam>
    public interface IContentFilter<TContent>
        : IMigrationHook<IEnumerable<ContentMigrationItem<TContent>>>
        where TContent : IContentReference
    { }
}
