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

using System.Linq;
using System.Net.Http.Headers;

namespace Tableau.Migration
{
    static internal class HttpHeaderExtensions
    {
        public static string? GetCorrelationId(this HttpHeaders header)
        {
            if (header != null && header.TryGetValues(Constants.REQUEST_CORRELATION_ID_HEADER, out var values))
            {
                return values.FirstOrDefault();
            }

            return null;
        }
    }
}
