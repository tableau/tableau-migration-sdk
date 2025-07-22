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

using System;

namespace Tableau.Migration
{
    /// <summary>
    /// Interface for an empty ID object that describes information on how to reference an item of content,
    /// for example through a Tableau API. 
    /// This in cases where the content type does not have a LUID on Tableau Server or Cloud.
    /// </summary>
    public interface IEmptyIdContentReference : IContentReference
    {
        /// <summary>
        /// Gets the empty unique identifier.
        /// </summary>
        new Guid Id => Guid.Empty;
    }
}