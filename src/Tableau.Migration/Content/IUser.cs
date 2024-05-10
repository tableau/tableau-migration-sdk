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

using Tableau.Migration.Api.Rest.Models;

namespace Tableau.Migration.Content
{
    /// <summary>
    /// Interface for a user content item.
    /// </summary>
    public interface IUser : IUsernameContent
    {
        /// <summary>
        /// Gets or sets the full name of the user.
        /// </summary>
        string FullName { get; set; }

        /// <summary>
        /// Gets or sets the email of the user.
        /// </summary>
        string Email { get; set; }

        /// <summary>
        /// Gets or sets the site role of the user.
        /// </summary>
        string SiteRole { get; set; }

        /// <summary>
        /// Gets or sets the authentication type of the user, 
        /// or null to not send an explicit authentication type for the user during migration.
        /// </summary>
        string? AuthenticationType { get; set; }

        /// <summary>
        /// Gets the user's administrator level derived from <see cref="SiteRole"/>.
        /// </summary>
        public string AdministratorLevel => SiteRoleMapping.GetAdministratorLevel(SiteRole);

        /// <summary>
        /// Gets the user's license level derived from <see cref="SiteRole"/>.
        /// </summary>
        public string LicenseLevel => SiteRoleMapping.GetLicenseLevel(SiteRole);

        /// <summary>
        /// Gets the user's publish capability derived from <see cref="SiteRole"/>.
        /// </summary>
        public bool CanPublish => SiteRoleMapping.GetPublishingCapability(SiteRole);
    }
}