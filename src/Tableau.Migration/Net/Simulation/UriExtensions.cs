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

namespace Tableau.Migration.Net.Simulation
{
    /// <summary>
    /// Static class containing extension methods for <see cref="Uri"/> objects.
    /// </summary>
    public static class UriExtensions
    {
        /// <summary>
        /// Returns the absolute path of the URI, trimmed of any final path separator.
        /// </summary>
        /// <param name="uri">The URI to get the trimmed path for.</param>
        /// <returns>The trimmed path.</returns>
        public static string TrimmedPath(this Uri uri) => uri.AbsolutePath.TrimEnd('/');
    }
}
