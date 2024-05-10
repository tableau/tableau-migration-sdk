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

using Tableau.Migration.Api;

namespace Tableau.Migration.Engine.Endpoints
{
    /// <summary>
    /// Interface for an object describing a source or destination endpoint defined in a <see cref="IMigrationPlan"/>.
    /// </summary>
    /// <param name="SiteConnectionConfiguration"><inheritdoc /></param>
    public record TableauApiEndpointConfiguration(TableauSiteConnectionConfiguration SiteConnectionConfiguration) : ITableauApiEndpointConfiguration
    {
        /// <summary>
        /// A <see cref="TableauApiEndpointConfiguration"/> with empty values, useful to detect if an endpoint has not yet been configured without using null.
        /// </summary>
        public static readonly TableauApiEndpointConfiguration Empty = new(TableauSiteConnectionConfiguration.Empty);

        /// <inheritdoc />
        public IResult Validate() => SiteConnectionConfiguration.Validate();
    }
}
