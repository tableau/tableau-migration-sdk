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
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Xunit.Sdk;

namespace Tableau.Migration.Tests
{
    public class ValuesAttribute<T> : DataAttribute
    {
        private readonly IImmutableList<T?> _values;

        public ValuesAttribute(IEnumerable<T?> values)
            : this(values.ToArray())
        { }

        public ValuesAttribute(params T?[] values)
        {
            _values = values.ToImmutableArray();
        }

        public override IEnumerable<object?[]> GetData(MethodInfo testMethod)
        {
            foreach (var value in GetValues())
                yield return CreateArguments(value);
        }

        protected virtual IEnumerable<T?> GetValues() => _values;

        protected virtual object?[] CreateArguments(T? value)
            => new object?[] { value };
    }

    public class ValuesAttribute : ValuesAttribute<object>
    {
        public ValuesAttribute(IEnumerable<object?> values)
            : base(values)
        { }

        public ValuesAttribute(params object?[] values)
            : base(values)
        { }
    }
}
