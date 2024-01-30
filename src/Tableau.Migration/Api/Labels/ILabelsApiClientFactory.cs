﻿// Copyright (c) 2023, Salesforce, Inc.
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

using Tableau.Migration.Content;

namespace Tableau.Migration.Api.Labels
{
    /// <summary>
    /// Interface for an object that can create <see cref="ILabelsApiClient{TContent}"/> objects.
    /// </summary>
    public interface ILabelsApiClientFactory
    {
        /// <summary>
        /// Returns a <see cref="ILabelsApiClient{TContent}"/>.
        /// </summary>        
        /// <returns></returns>
        ILabelsApiClient<TContent> Create<TContent>()
            where TContent : IContentReference, IWithLabels;
    }
}
