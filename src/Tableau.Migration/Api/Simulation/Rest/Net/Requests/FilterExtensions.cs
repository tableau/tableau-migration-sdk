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
using System.Linq;
using Tableau.Migration.Net.Rest.Filtering;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Requests
{
    internal static class FilterExtensions
    {
        public static Filter? GetFilter(this IEnumerable<Filter> filters, string field, string? @operator = null)
        {
            return filters
                .LastOrDefault(f =>
                    f.Field == field &&
                    (@operator is null || f.Operator == @operator));
        }

        public static string? GetFilterValue(this IEnumerable<Filter> filters, string field, string? @operator = null)
            => filters.GetFilter(field, @operator)?.Value.ToString();
    }
}
