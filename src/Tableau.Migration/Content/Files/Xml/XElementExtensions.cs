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

using System.Collections.Generic;
using System.Xml.Linq;

namespace Tableau.Migration.Content.Files.Xml
{
    /// <summary>
    /// Static class containing <see cref="XElement"/> extension methods to assist with
    /// Tableau XML format's Feature Forked Subtree (FFS) mangling.
    /// </summary>
    public static class XElementExtensions
    {
        /// <summary>
        /// Gets all attributes that match (without FFS mangling considered) a given name.
        /// </summary>
        /// <param name="el">The element to get attributes for.</param>
        /// <param name="unmangledName">The unmangled/non-FFS name to search for.</param>
        /// <returns>The attributes with the given name.</returns>
        public static IEnumerable<XAttribute> GetFeatureFlaggedAttributes(this XElement el, XName unmangledName)
        {
            foreach (var atr in el.Attributes())
            {
                if (atr.Name.MatchFeatureFlagName(unmangledName))
                {
                    yield return atr;
                }
            }
        }
    }
}
