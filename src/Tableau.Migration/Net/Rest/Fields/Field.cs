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

namespace Tableau.Migration.Net.Rest.Fields
{
    /// <summary>
    /// <para>
    /// Class representing a REST API field
    /// </para>
    /// <para>
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_concepts_fields.htm">Tableau API Reference</see> for more details.
    /// </para>
    /// </summary>
    public class Field
    {
        /// <summary>
        /// <para>
        /// Gets a <see cref="Field"/> representing "_all_" fields.
        /// </para>
        /// <para>
        /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_concepts_fields.htm#field-expression-syntax">Tableau API Reference</see> for more details.
        /// </para>
        /// </summary>
        public static Field All { get; } = new("_all_");

        /// <summary>
        /// <para>
        /// Gets a <see cref="Field"/> representing "_default_" fields.
        /// </para>
        /// <para>
        /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_concepts_fields.htm#field-expression-syntax">Tableau API Reference</see> for more details.
        /// </para>
        /// </summary>
        public static Field Default { get; } = new("_default_");

        /// <summary>
        /// Gets the field name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the field's expression for use in query strings.
        /// </summary>
        public string Expression { get; }

        /// <summary>
        /// Creates a new <see cref="Field"/> instance.
        /// </summary>
        /// <param name="name">The field name</param>
        public Field(string name)
        {
            Name = name;
            Expression = Name;
        }
    }
}
