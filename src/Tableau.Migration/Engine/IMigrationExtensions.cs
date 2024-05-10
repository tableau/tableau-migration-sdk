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

using System.Diagnostics.CodeAnalysis;
using Tableau.Migration.Engine.Endpoints;

namespace Tableau.Migration.Engine
{
    internal static class IMigrationExtensions
    {
        public static bool TryGetSourceApiEndpoint(this IMigration migration, [NotNullWhen(true)] out ISourceApiEndpoint? apiEndpoint)
        {
            apiEndpoint = null;

            if (migration.Source is ISourceApiEndpoint apiSourceEndpoint)
            {
                apiEndpoint = apiSourceEndpoint;
                return true;
            }

            return false;
        }

        public static bool TryGetDestinationApiEndpoint(this IMigration migration, [NotNullWhen(true)] out IDestinationApiEndpoint? apiEndpoint)
        {
            apiEndpoint = null;

            if (migration.Destination is IDestinationApiEndpoint apiDestinationEndpoint)
            {
                apiEndpoint = apiDestinationEndpoint;
                return true;
            }

            return false;
        }
    }
}
