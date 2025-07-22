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

namespace Tableau.Migration.Api.Rest
{
    internal static class RestErrorCodes
    {
        public const string BAD_REQUEST = "400000";

        public const string CREATE_PROJECT_FORBIDDEN = "403045";

        public const string GENERIC_CREATE_SUBSCRIPTION_ERROR = "400063";

        public const string GENERIC_QUERY_JOB_ERROR = "400031";

        public const string GROUP_NAME_CONFLICT_ERROR_CODE = "409009";

        public const string GROUP_SET_NAME_CONFLICT_ERROR_CODE = "409120";

        public const string INVALID_CAPABILITY_FOR_RESOURCE = "400009";

        public const string LOGIN_ERROR = "401001";

        public const string PROJECT_NAME_CONFLICT_ERROR_CODE = "409006";

        public const string SITES_QUERY_NOT_SUPPORTED = "403069";

        public const string CUSTOM_VIEW_ALREADY_EXISTS = "403166";

        public const string FEATURE_DISABLED = "403157";

        public static bool Equals(string? x, string? y) => string.Equals(x, y, StringComparison.Ordinal);

    }
}
