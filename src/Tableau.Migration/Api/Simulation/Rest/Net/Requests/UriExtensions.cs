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
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using Tableau.Migration.Net;
using Tableau.Migration.Net.Rest;
using Tableau.Migration.Net.Rest.Filtering;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Requests
{
    internal static class UriExtensions
    {
        private static readonly StringComparer _comparer = StringComparer.OrdinalIgnoreCase;

        public static string? GetSegment(this Uri? uri, int index)
        {
            var segments = uri?.EnsureAbsoluteUri().GetNonSlashSegments().ToArray() ?? Array.Empty<string>();

            if (segments.Length < index + 1)
                return null;

            return segments[index];
        }

        public static string? GetLastSegment(this Uri? uri)
            => uri?.Segments.LastOrDefault()?.TrimPaths();

        public static string? GetNextToLastSegment(this Uri? uri)
            => uri?.Segments?[^2]?.TrimPaths();

        public static Guid GetRequestIdFromUri(this Uri? uri, bool hasSuffix = false)
        {
            var idSegment = !hasSuffix ? uri.GetLastSegment() : uri.GetNextToLastSegment();

            return idSegment is not null
                && Guid.TryParse(idSegment, out var id)
                ? id : Guid.Empty;
        }

        public static Guid? GetSiteIdFromUrl(this Uri? uri) => GetIdAfterSegment(uri, "sites");

        public static Guid? GetProjectIdFromUrl(this Uri? uri) => GetIdAfterSegment(uri, "projects");

        public static Guid? GetIdAfterSegment(this Uri? uri, string segmentBeforeId)
        {
            var getNextSegment = false;

            foreach (var segment in uri?.EnsureAbsoluteUri().GetNonSlashSegments() ?? Array.Empty<string>())
            {
                if (getNextSegment)
                {
                    if (Guid.TryParse(segment, out var id))
                        return id;

                    return null;
                }

                if (StringComparer.OrdinalIgnoreCase.Equals(segment, segmentBeforeId))
                {
                    getNextSegment = true;
                }
            }

            return null;
        }

        public static bool IsDefaultPermissionsRequest(this Uri? uri)
            => _comparer.Equals("default-permissions", uri.GetSegment(6));

        public static string ParseDefaultPermissionsContentType(this Uri? uri)
        {
            if (!uri.IsDefaultPermissionsRequest())
                throw new ArgumentException("The specified URI is not a default permissions URI.", nameof(uri));

            var segment = uri.GetSegment(7);

            if (!string.IsNullOrWhiteSpace(segment))
                return segment;

            throw new ArgumentException("Could not determine the default permissions content type from the request.", nameof(uri));
        }

        public static IImmutableList<Filter> ParseFilters(this Uri? uri)
        {
            var filterValue = uri.GetQueryStringValue("filter");

            if (string.IsNullOrEmpty(filterValue))
                return ImmutableArray<Filter>.Empty;

            // We're using a dictionary with ordering to maintain field order and handle duplicate fields.
            var filtersByField = new Dictionary<string, (int Order, Filter Filter)>(StringComparer.Ordinal);

            var filters = filterValue.Trim().Split(',');

            var index = 0;

            foreach (var filter in filters)
            {
                var filterParts = filter.Split(':', 3);

                if (filterParts.Length != 3)
                    continue;

                // Replace the existing field name's filter if it exists (last one wins)
                // https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_concepts_filtering_and_sorting.htm#filter-expression-notes
                filtersByField[filterParts[0]] = (++index, new Filter(filterParts[0], filterParts[1], filterParts[2]));
            }

            return filtersByField.Values.OrderBy(v => v.Order).Select(v => v.Filter).ToImmutableArray();
        }

        public static string GetQueryStringValue(this Uri? uri, string key)
        {
            if (string.IsNullOrWhiteSpace(uri?.Query))
            {
                return string.Empty;
            }

            var queryRegex = new Regex($"{key}=(?<value>[^&]*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            var matches = queryRegex.Matches(uri.Query);
            if (matches.Count > 0)
            {
                return matches[0].Groups["value"].Value.UrlDecode();
            }

            return string.Empty;
        }
    }
}
