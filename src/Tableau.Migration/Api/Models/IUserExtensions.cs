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

using System.Text;
using Tableau.Migration.Content;

namespace Tableau.Migration.Api.Models
{
    /// <summary>
    /// Extension methods for <see cref="IUser"/>.
    /// </summary>
    internal static class IUserExtensions
    {
        /// <summary>
        /// Converts the User object into a comma separated string to be used with the user_import call.
        /// The order of elements is important. 
        /// See https://help.tableau.com/current/server/en-us/csvguidelines.htm for guidelines.
        /// </summary>
        internal static void AppendCsvLine(this IUser user, StringBuilder builder)
        {
            var siteRole = Guard.AgainstNullEmptyOrWhiteSpace(user.SiteRole, () => user.SiteRole);
            builder.AppendCsvLine(
                Guard.AgainstNullEmptyOrWhiteSpace(user.Name, () => user.Name),
                string.Empty, //Password (is intentionally empty)
                user.FullName,
                user.LicenseLevel,
                user.AdministratorLevel,
                user.CanPublish.ToString(),
                user.Email);
        }
    }
}
