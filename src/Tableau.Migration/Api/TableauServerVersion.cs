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

namespace Tableau.Migration.Api
{
    /// <summary>
    /// Object containing version information for a Tableau server.
    /// </summary>
    /// <param name="RestApiVersion">The server's REST API version.</param>
    /// <param name="ProductVersion">The server's product version.</param>
    /// <param name="BuildVersion">The server's build version.</param>
    public readonly record struct TableauServerVersion(string RestApiVersion, string ProductVersion, string BuildVersion)
    { }
}
