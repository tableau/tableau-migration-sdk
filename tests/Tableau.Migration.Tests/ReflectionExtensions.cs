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
using System.Collections.Immutable;
using System.Reflection;

namespace Tableau.Migration.Tests
{
    public static class ReflectionExtensions
    {
        public static object? GetFieldValue(this Type type, string fieldName, object obj)
        {
            return type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(obj);
        }

        public static object? GetFieldValue(this object obj, string fieldName)
        {
            return GetFieldValue(obj.GetType(), fieldName, obj);
        }

        public static object? GetFieldValue(this object obj, Type type, string fieldName)
        {
            return GetFieldValue(type, fieldName, obj);
        }

        public static object? GetFieldValue(this Type type, string fieldName)
        {
            return type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static)!.GetValue(null);
        }

        public static TValue? GetFieldValue<TValue>(this Type type, string fieldName)
            => (TValue?)type.GetFieldValue(fieldName);

        public static object? GetPropertyValue(this Type type, string fieldName, object obj)
        {
            return type.GetProperty(fieldName, BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(obj);
        }

        public static object? GetPropertyValue(this object obj, string fieldName)
        {
            return GetPropertyValue(obj.GetType(), fieldName, obj);
        }

        public static object? GetPropertyValue(this object obj, Type type, string fieldName)
        {
            return GetPropertyValue(type, fieldName, obj);
        }

        public static object? GetPropertyValue(this Type type, string fieldName)
        {
            return type.GetProperty(fieldName, BindingFlags.NonPublic | BindingFlags.Static)!.GetValue(null);
        }

        public static TValue? GetPropertyValue<TValue>(this Type type, string fieldName)
            => (TValue?)type.GetPropertyValue(fieldName);

        public static bool HasGenericTypeDefinition(this Type type, Type genericTypeDefinition)
        {
            if (!genericTypeDefinition.IsGenericTypeDefinition)
                throw new ArgumentException($"Type {genericTypeDefinition.FullName} is not a generic type definition.");

            return type.IsConstructedGenericType && type.GetGenericTypeDefinition() == genericTypeDefinition;
        }

        public static bool IsConcrete(this Type type) => !type.IsInterface && !type.IsAbstract;

        public static bool IsAssignableToAny(this Type type, params Type[] targetTypes)
        {
            foreach (var targetType in targetTypes)
            {
                if (type.IsAssignableTo(targetType))
                    return true;
            }

            return false;
        }

        public static bool IsAssignableFromAny(this Type type, params Type[] targetTypes)
        {
            foreach (var targetType in targetTypes)
            {
                if (type.IsAssignableFrom(targetType))
                    return true;
            }

            return false;
        }

        public static IImmutableList<Type> GetBaseTypes(this Type type)
        {
            var baseTypes = ImmutableArray.CreateBuilder<Type>();

            var baseType = type.BaseType;

            while (baseType is not null)
            {
                baseTypes.Add(baseType);
                baseTypes.AddRange(baseType.GetBaseTypes());

                baseType = baseType.BaseType;
            }

            return baseTypes.ToImmutable();
        }
    }
}
