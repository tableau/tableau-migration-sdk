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

namespace Tableau.Migration.Api.Rest.Models.Types
{
    /// <summary>
    /// Class containing week day constants.
    /// </summary>
    public class WeekDays : StringEnum<WeekDays>
    {
        /// <summary>
        /// Gets the name of the Sunday week day.
        /// </summary>
        public const string Sunday = "Sunday";

        /// <summary>
        /// Gets the name of the Monday week day.
        /// </summary>
        public const string Monday = "Monday";

        /// <summary>
        /// Gets the name of the Tuesday week day.
        /// </summary>
        public const string Tuesday = "Tuesday";

        /// <summary>
        /// Gets the name of the Wednesday week day.
        /// </summary>
        public const string Wednesday = "Wednesday";

        /// <summary>
        /// Gets the name of the Thursday week day.
        /// </summary>
        public const string Thursday = "Thursday";

        /// <summary>
        /// Gets the name of the Friday week day.
        /// </summary>
        public const string Friday = "Friday";

        /// <summary>
        /// Gets the name of the Saturday week day.
        /// </summary>
        public const string Saturday = "Saturday";
    }
}
