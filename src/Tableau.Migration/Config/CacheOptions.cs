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

namespace Tableau.Migration.Config
{
    /// <summary>
    /// Class for configuration settings specific to a particular cache.
    /// </summary>
    public class CacheOptions
    {
        /// <summary>
        /// Defaults for cache options.
        /// </summary>
        public static class Defaults
        {
            /// <summary>
            /// The default cache size limit
            /// </summary>
            public static readonly long? SIZE_LIMIT = null;
        }

        /// <summary>
        /// Gets or sets the cache size limit, or null to not limit cache by size.
        /// </summary>
        public long? SizeLimit
        {
            get => _sizeLimit ?? Defaults.SIZE_LIMIT;
            set => _sizeLimit = value;
        }
        private long? _sizeLimit;
    }
}
