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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Tableau.Migration.Net.Simulation
{
    internal sealed class BaseUrlComparer : IEqualityComparer<Uri>
    {
        public static BaseUrlComparer Instance = new();

        public bool Equals(Uri? x, Uri? y)
        {
            if (x is null && y is null)
                return true;

            if (x is null || y is null)
                return false;

            return GetBaseUri(x).Equals(GetBaseUri(y));
        }

        public int GetHashCode([DisallowNull] Uri obj) => GetBaseUri(obj).GetHashCode();

        private static Uri GetBaseUri(Uri uri)
            => new(uri.GetComponents(UriComponents.SchemeAndServer, UriFormat.UriEscaped).ToLower());
    }
}
