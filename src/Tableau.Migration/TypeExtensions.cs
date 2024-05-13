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

namespace Tableau.Migration
{
    /// <summary>
    /// Static class containing extension methods for <see cref="Type"/> objects.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Gets all the properites of an interface.
        /// <see cref="Type.GetProperties()"/> will get all the properties of a class, base and inherited. However, for interfaces it only gets
        /// the extended properties.
        /// This method will recusively find all the properties of the inherited interfaces and aggregate them.
        /// Calling this overload is equivalent to calling the GetAllInterfaceProperties(BindingFlags) overload with a bindingAttr argument equal to BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public
        /// </summary>
        /// <param name="interfaceType">The interface type. Ex: typeof(IMyInterface)</param>
        public static PropertyInfo[] GetAllInterfaceProperties(this Type interfaceType)
        {
            return GetAllInterfaceProperties(interfaceType, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public).ToArray();
        }

        /// <summary>
        /// Gets all the properites of an interface.
        /// <see cref="Type.GetProperties()"/> will get all the properties of a class, base and inherited. However, for interfaces it only gets
        /// the extended properties.
        /// This method will recusively find all the properties of the inherited interfaces and aggregate them.
        /// </summary>
        /// <param name="interfaceType">The interface type. Ex: typeof(IMyInterface)</param>
        /// <param name="bindingAttr"><see cref="BindingFlags"/> to use.</param>
        public static PropertyInfo[] GetAllInterfaceProperties(this Type interfaceType, BindingFlags bindingAttr = BindingFlags.Default)
        {
            return FindAllInterfaceProperties(interfaceType, bindingAttr).ToArray();
        }

        /// <summary>
        /// Gets all the methods of an interface.
        /// <see cref="Type.GetMethods()"/> will get all the methods of a class, base and inherited. However, for interfaces it only gets
        /// the extended methods.
        /// This method will recusively find all the methods of the inherited interfaces and aggregate them.
        /// Calling this overload is equivalent to calling the GetAllInterfaceMethods(BindingFlags) overload with a bindingAttr argument equal to BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public
        /// </summary>
        /// <param name="interfaceType">The interface type. Ex: typeof(IMyInterface)</param>
        public static MethodInfo[] GetAllInterfaceMethods(this Type interfaceType)
        {
            return GetAllInterfaceMethods(interfaceType, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public).ToArray();
        }

        /// <summary>
        /// Gets all the methods of an interface.
        /// <see cref="Type.GetMethods()"/> will get all the methods of a class, base and inherited. However, for interfaces it only gets
        /// the extended methods.
        /// This method will recusively find all the methods of the inherited interfaces and aggregate them.
        /// </summary>
        /// <param name="interfaceType">The interface type. Ex: typeof(IMyInterface)</param>
        /// <param name="bindingAttr"><see cref="BindingFlags"/> to use.</param>
        public static MethodInfo[] GetAllInterfaceMethods(this Type interfaceType, BindingFlags bindingAttr = BindingFlags.Default)
        {
            return FindAllInterfaceMethods(interfaceType, bindingAttr).ToArray();
        }

        #region - private implementations 

        private static List<PropertyInfo> FindAllInterfaceProperties(this Type interfaceType, BindingFlags bindingAttr = BindingFlags.Default)
        {
            if (!interfaceType.IsInterface)
            {
                throw new ArgumentException($"{interfaceType} must be an interface", nameof(interfaceType));
            }

            var props = interfaceType.GetProperties(bindingAttr).ToList();

            var interfaces = interfaceType.GetInterfaces();
            foreach (var i in interfaces)
            {
                props.AddRange(FindAllInterfaceProperties(i, bindingAttr));
            }

            return props;
        }

        private static List<MethodInfo> FindAllInterfaceMethods(this Type interfaceType, BindingFlags bindingAttr = BindingFlags.Default)
        {
            if (!interfaceType.IsInterface)
            {
                throw new ArgumentException($"{interfaceType} must be an interface", nameof(interfaceType));
            }

            var props = interfaceType.GetMethods(bindingAttr).ToList();

            var interfaces = interfaceType.GetInterfaces();
            foreach (var i in interfaces)
            {
                props.AddRange(FindAllInterfaceMethods(i, bindingAttr));
            }

            return props;
        }

        #endregion
    }
}
