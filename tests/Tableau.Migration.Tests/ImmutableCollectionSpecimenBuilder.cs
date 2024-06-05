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
using System.Collections.Immutable;
using AutoFixture.Kernel;

namespace Tableau.Migration.Tests
{
    internal sealed class ImmutableCollectionSpecimenBuilder : ISpecimenBuilder
    {
        private static Type? GetConcreteImmutableType(Type genericType)
        {
            if (genericType == typeof(IImmutableList<>))
            {
                return typeof(ImmutableList<>);
            }

            return null;
        }

        public object Create(object request, ISpecimenContext context)
        {
            if(!(request is Type t) || !t.IsGenericType)
            {
                return new NoSpecimen();
            }

            var typeArguments = t.GetGenericArguments();
            var genericType = t.GetGenericTypeDefinition();

            if(genericType == typeof(ImmutableList<>) || genericType == typeof(IImmutableList<>))
            {
                dynamic list = context.Resolve(typeof(List<>).MakeGenericType(typeArguments));
                return ImmutableList.ToImmutableList(list);
            }

            return new NoSpecimen();
        }
    }
}
