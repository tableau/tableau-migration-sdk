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
using System.ComponentModel.DataAnnotations;

namespace Tableau.Migration.Api
{
    /// <summary>
    /// Object that describes how to connect to a <see cref="IApiClient"/>.
    /// </summary>
    /// <param name="ServerUrl">The base URL of the Tableau Server, or the Tableau Cloud URL to connect to.</param>
    /// <param name="SiteContentUrl">The content URL of the site to connect to. Can be empty string for default site.</param>
    /// <param name="AccessTokenName">The name of the personal access token to use to sign into the site.</param>
    /// <param name="AccessToken">The personal access token to use to sign into the site.</param>
    public readonly record struct TableauSiteConnectionConfiguration(
        Uri ServerUrl,
        string SiteContentUrl,
        [property: Required] string AccessTokenName,
        [property: Required] string AccessToken
    )
    {
        /// <summary>
        /// A <see cref="TableauSiteConnectionConfiguration"/> with empty values, useful to detect if a value has not yet been configured without using null.
        /// </summary>
        public static readonly TableauSiteConnectionConfiguration Empty = new(new Uri("https://localhost"), string.Empty, string.Empty, string.Empty);

        /// <summary>
        /// Validates that the connection configuration has enough information to connect.
        /// </summary>
        /// <returns>The validation result.</returns>
        public IResult Validate() => this.ValidateSimpleProperties();
    }
}
