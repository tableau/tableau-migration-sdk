// Copyright (c) 2023, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the ""License"") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an ""AS IS"" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using System;
using System.Collections.Generic;
using System.Linq;

namespace Tableau.Migration.Interop
{
    /// <summary>
    /// Helper class for interop work
    /// </summary>
    public static class InteropHelper
    {
        internal static List<string> ignoredMethods = new()
        {
            "GetHashCode",
            "ToString",
            "Deconstruct",
            "GetType",
            "Equals",
            "GetTypeCode",
            "CompareTo",
            "HasFlag"
        };

        /// <summary>
        /// Gets methods of a class that aren't "generic"
        /// </summary>
        /// <typeparam name="T">The objects type to get methods from</typeparam>
        /// <returns>List of method names</returns>
        public static List<string> GetMethods<T>()
        {
            List<string> methods;
            List<string> ret = new();

            if (typeof(T).IsInterface)
            {
                methods = typeof(T).GetAllInterfaceMethods().Select(p => p.Name).ToList();
            }
            else
            {
                methods = typeof(T).GetMethods().Select(p => p.Name).ToList();
            }

            foreach (var method in methods)
            {
                if (!method.StartsWith("get_") && // getters
                   !method.StartsWith("set_") && // setters
                   !method.StartsWith("op_") &&  // operators 
                   !ignoredMethods.Contains(method)
                   )
                {
                    ret.Add(method);
                }
            }

            return ret;
        }

        /// <summary>
        /// Gets the properies of a class
        /// </summary>
        /// <typeparam name="T">The objects type to get properties from</typeparam>
        /// <returns>List of property names</returns>
        public static List<string> GetProperties<T>()
        {
            if (typeof(T).IsInterface)
            {
                return typeof(T).GetAllInterfaceProperties().Select(p => p.Name).ToList();
            }
            else
            {
                return typeof(T).GetProperties().Select(p => p.Name).ToList();
            }
        }


        /// <summary>
        /// Gets all the names and values of a enum and returns them as a list.
        /// </summary>
        /// <typeparam name="T">The enum type</typeparam>
        /// <returns>List of tuples with all the names and values</returns>
        static public List<Tuple<string, object>> GetEnum<T>() where T : Enum
        {
            var ret = new List<Tuple<string, object>>();

            var enumType = typeof(T);
            var underlyingType = Enum.GetUnderlyingType(enumType);

            var names = Enum.GetNames(enumType);

            foreach (var name in names)
            {
                var value = Enum.Parse(enumType, name);
                object underlyingValue = Convert.ChangeType(value, underlyingType);

                ret.Add(new Tuple<string, object>(name, underlyingValue));
            }

            return ret;
        }
    }
}
