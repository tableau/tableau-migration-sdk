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

namespace Tableau.Migration.Content
{
    /// <summary>
    /// Interface for a content item that has metadata around publishing information.
    /// </summary>
    public interface IPublishedContent
    {
        /// <summary>
        /// Gets the created timestamp.
        /// </summary>
        string CreatedAt { get; }

        /// <summary>
        /// Gets the updated timestamp.
        /// </summary>
        string? UpdatedAt { get; }

        /// <summary>
        /// Gets the webpage URL.
        /// </summary>
        string? WebpageUrl { get; }
    }
}
