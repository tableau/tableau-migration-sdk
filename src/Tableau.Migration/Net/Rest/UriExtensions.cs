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
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Tableau.Migration.Net.Rest
{
    internal static class UriExtensions
    {
        public static bool IsRest([NotNullWhen(true)] this Uri? uri)
        {
            if (uri is null)
                return false;

            var absoluteUri = uri.EnsureAbsoluteUri();

            var segments = absoluteUri.GetNonSlashSegments();

            // At least /api/<version>
            if (segments.Length < 2)
                return false;

            return segments[0].Equals("api", StringComparison.OrdinalIgnoreCase) is true;
        }

        public static bool IsRestSignIn([NotNullWhen(true)] this Uri? uri)
        {
            if (!uri.IsRest())
                return false;

            return uri.EnsureAbsoluteUri().GetNonSlashSegments().Skip(2).Take(2).SequenceEqual(new[] { "auth", "signin" }, StringComparer.OrdinalIgnoreCase) is true;
        }

        public static bool IsRestSignOut([NotNullWhen(true)] this Uri? uri)
        {
            if (!uri.IsRest())
                return false;

            return uri.EnsureAbsoluteUri().GetNonSlashSegments().Skip(2).Take(2).SequenceEqual(new[] { "auth", "signout" }, StringComparer.OrdinalIgnoreCase) is true;
        }

        internal static Uri EnsureAbsoluteUri(this Uri uri)
            => uri.IsAbsoluteUri ? uri : new Uri(new Uri("https://localhost"), uri.ToString());

        internal static string[] GetNonSlashSegments(this Uri uri)
            => uri.Segments.Where(s => s != "/").Select(s => s.TrimPaths()).ToArray();
    }
}
