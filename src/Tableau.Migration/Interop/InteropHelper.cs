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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tableau.Migration.Content;

namespace Tableau.Migration.Interop
{
    /// <summary>
    /// Helper class for interop work
    /// </summary>
    public static class InteropHelper
    {
        internal static HashSet<string> _ignoredMethods = new()
        {
            nameof(object.GetHashCode),
            nameof(object.ToString),
            nameof(object.GetType),
            nameof(object.Equals),
            nameof(Enum.HasFlag),
            "Deconstruct",
            "GetTypeCode",
            "CompareTo",
            "<Clone>$"
        };

        //DNG modify this to be less naive
        //- Find if the generic has a Constraint, then build the types with that generic
        private static Type MakeSampleGenericType(Type generic)
        {
            var genericTypes = generic.GetGenericArguments()
                .Select(gt =>
                {
                    var constraints = gt.GetGenericParameterConstraints();

                    if (constraints.Length == 0)
                    {
                        return typeof(IUser);
                    }
                    
                    return constraints.First();
                })
                .ToArray();

            return generic.MakeGenericType(genericTypes);
        }
        
        private static bool IsPropertyMethod(MethodInfo method)
            => method.Name.StartsWith("get_") || method.Name.StartsWith("set_");

        private static bool IsOperator(MethodInfo method)
            => method.Name.StartsWith("op_");

        /// <summary>
        /// Gets the methods of a class.
        /// </summary>
        /// <typeparam name="T">The type to get methods from.</typeparam>
        /// <returns>The method names.</returns>
        public static IEnumerable<string> GetMethods<T>()
            => GetMethods(typeof(T));

        /// <summary>
        /// Gets the methods of a class.
        /// </summary>
        /// <param name="type">The type to get methods from.</param>
        /// <returns>The method names.</returns>
        public static IEnumerable<string> GetMethods(Type type)
        {
            if(type.ContainsGenericParameters)
            {
                return GetMethods(MakeSampleGenericType(type));
            }

            var methods = type.IsInterface ? type.GetAllInterfaceMethods() : type.GetMethods();

            return methods
                .Where(m => !IsPropertyMethod(m))
                .Where(m => !IsOperator(m))
                .Where(m => !_ignoredMethods.Contains(m.Name))
                .Select(m => m.Name);
        }

        /// <summary>
        /// Gets the properties of a class.
        /// </summary>
        /// <typeparam name="T">The type to get properties from.</typeparam>
        /// <returns>The property names.</returns>
        public static IEnumerable<string> GetProperties<T>()
            => GetProperties(typeof(T));

        /// <summary>
        /// Gets the properties of a class.
        /// </summary>
        /// <param name="type">The type to get properties from.</param>
        /// <returns>The property names.</returns>
        public static IEnumerable<string> GetProperties(Type type)
        {
            if (type.ContainsGenericParameters)
            {
                return GetProperties(MakeSampleGenericType(type));
            }

            var properties = type.IsInterface ? type.GetAllInterfaceProperties() : type.GetProperties();

            return properties.Select(p => p.Name);
        }

        /// <summary>
        /// Gets all the names and values of an enumeration.
        /// </summary>
        /// <typeparam name="T">The enum type.</typeparam>
        /// <returns>Tuples with all the names and values.</returns>
        public static IEnumerable<Tuple<string, object>> GetEnum<T>() 
        {
            var enumType = typeof(T);
            if (enumType.BaseType == typeof(Enum))
            {
                var underlyingType = Enum.GetUnderlyingType(enumType);

                foreach (var name in Enum.GetNames(enumType))
                {
                    var value = Enum.Parse(enumType, name);
                    object underlyingValue = Convert.ChangeType(value, underlyingType);

                    yield return new(name, underlyingValue);
                }
            }
            else if(enumType.BaseType == typeof(StringEnum<T>))
            {
                foreach(var field in enumType.GetFields(BindingFlags.Public | BindingFlags.Static))
                {
                    if (field is null || !field.IsLiteral || field.IsInitOnly)
                        continue;

                    yield return new(field.Name, field.GetValue(null)!);
                }
            }
        }
    }
}
