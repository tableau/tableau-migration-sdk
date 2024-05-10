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

namespace Tableau.Migration.Content
{
    /// <summary>
    /// Interface for the current server session information.
    /// </summary>
    public interface IServerSession
    {
        /// <summary>
        /// Gets the current session's site.
        /// </summary>
        IContentReference Site { get; }

        /// <summary>
        /// Gets the site settings, or null if the user does not have access to the settings.
        /// </summary>
        ISiteSettings? Settings { get; }

        /// <summary>
        /// Gets whether or not the current user has administrator access.
        /// </summary>
        [MemberNotNullWhen(true, nameof(Settings))]
        bool IsAdministrator { get; }
    }
}
