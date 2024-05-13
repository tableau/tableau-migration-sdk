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
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Moq;

namespace Tableau.Migration.Tests.Reflection
{
    public class ObjectReflectionWrapper<T>
        where T : notnull
    {
        private static readonly ConcurrentDictionary<T, ObjectReflectionWrapper<T>> _cache = new();

        private readonly TypeReflectionWrapper<T> _typeWrapper = TypeReflectionWrapper<T>.Instance;

        public readonly T Object;

        public ObjectReflectionWrapper(T @object)
        {
            Object = @object;
        }

        public object? GetFieldValue(string fieldName) => _typeWrapper.GetFieldValue<object?>(Object, fieldName);

        public TValue GetFieldValue<TValue>(string fieldName) => _typeWrapper.GetFieldValue<TValue>(Object, fieldName);

        public Mock<TValue> GetMockFieldValue<TValue>(string fieldName)
            where TValue : class
            => _typeWrapper.GetMockFieldValue<TValue>(Object, fieldName);

        public Mock<TValue> GetMockFieldValue<TValue>(Func<ObjectReflectionWrapper<T>, TValue> getFieldValue)
            where TValue : class
            => Mock.Get(getFieldValue(this));

        public static ObjectReflectionWrapper<T> InstanceFor(T @object)
            => _cache.GetOrAdd(@object, _ => new ObjectReflectionWrapper<T>(@object));
    }

    public abstract class ObjectReflectionWrapper<TWrapper, TType> : ObjectReflectionWrapper<TType>
        where TWrapper : ObjectReflectionWrapper<TWrapper, TType>
        where TType : notnull
    {
        private static readonly ConcurrentDictionary<TType, TWrapper> _cache = new();

        public ObjectReflectionWrapper(TType @object)
            : base(@object)
        { }

        public Mock<TValue>? GetMockFieldValue<TValue>(Func<TWrapper, TValue?> getFieldValue, [DoesNotReturnIf(false)] bool canBeNull = false)
            where TValue : class
        {
            var value = getFieldValue((TWrapper)this);

            if (value is null)
            {
                if (canBeNull)
                    return null;
                else
                    throw new ArgumentException($"The value returned from {nameof(getFieldValue)} is null.", nameof(getFieldValue));
            }

            return Mock.Get(value);
        }

        public TMock? GetMockFieldValue<TMock, TValue>(Func<TWrapper, TValue?> getFieldValue, [DoesNotReturnIf(false)] bool canBeNull = false)
            where TMock : Mock<TValue>
            where TValue : class
            => (TMock?)GetMockFieldValue(getFieldValue, canBeNull);

        new public static TWrapper InstanceFor(TType @object)
            => _cache.GetOrAdd(@object, _ => (TWrapper)Activator.CreateInstance(typeof(TWrapper), @object)!);
    }
}
