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
using Moq;

namespace Tableau.Migration.Tests.Reflection
{
    public class TypeReflectionWrapper<T>
    {
        public static readonly TypeReflectionWrapper<T> Instance = new();

        private readonly Lazy<IImmutableDictionary<string, FieldInfo>> _fields;

        private readonly Func<object, FieldInfo, object?> _fieldValueFactory;

        protected readonly Type Type;

        public TypeReflectionWrapper()
        {
            Type = typeof(T);

            _fields = new(GetFields);

            _fieldValueFactory = (@object, field) => field.GetValue(@object);
        }

        private ImmutableSortedDictionary<string, FieldInfo> GetFields()
            => Type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .ToImmutableSortedDictionary(f => f.Name, f => f);

        protected object? GetFieldValue(object @object, string fieldName)
        {
            if (_fields.Value.TryGetValue(fieldName, out var field))
                return _fieldValueFactory(@object, field);

            var fields = String.Join(Environment.NewLine, _fields.Value.Keys);

            throw new ArgumentException($"Field \"{fieldName}\" was not found on type {Type.Name}. Available fields:{Environment.NewLine}{fields}", nameof(fieldName));
        }

        public TValue GetFieldValue<TValue>(object @object, string fieldName) => (TValue)GetFieldValue(@object, fieldName)!;

        public Mock<TValue> GetMockFieldValue<TValue>(object @object, string fieldName)
            where TValue : class
        {
            var value = (TValue?)GetFieldValue(@object, fieldName) ??
                throw new ArgumentException($"The value for field \"{fieldName}\" on type {Type.Name} is null.", nameof(fieldName));

            return Mock.Get(value);
        }

        public static ObjectReflectionWrapper<TType> ForObject<TType>(TType @object)
            where TType : notnull
            => new(@object);
    }
}
