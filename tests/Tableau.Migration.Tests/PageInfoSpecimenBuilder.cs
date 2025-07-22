//
//  Copyright (c) 2025, Salesforce, Inc.
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
using System.Linq;
using System.Reflection;
using AutoFixture.Kernel;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Paging;

namespace Tableau.Migration.Tests
{
    /// <summary>
    /// <see cref="ISpecimenBuilder"/> implementation that creates realistic values for <see cref="IPageInfo"/> implementations.
    /// </summary>
    /// <remarks>
    /// This builder is important so that the default fixture values generated with paging information
    /// is "realistic" enough to not cause infinite loops.
    /// </remarks>
    internal sealed class PageInfoSpecimenBuilder : ISpecimenBuilder
    {
        private const int DEFAULT_PAGE_NUMBER = 1;
        private const int DEFAULT_PAGE_SIZE = 1000;

        private static bool IsPagedResponseType(Type t)
            => t.GetBaseTypes().Any(b => b.IsGenericType && b.GetGenericTypeDefinition() == typeof(PagedTableauServerResponse<>));

        public object Create(object request, ISpecimenContext context)
        {
            if (request is not Type t)
            {
                return new NoSpecimen();
            }

            if (t == typeof(Pagination))
            {
                return new Pagination
                {
                    PageNumber = DEFAULT_PAGE_NUMBER,
                    PageSize = DEFAULT_PAGE_SIZE,
                    TotalAvailable = DEFAULT_PAGE_SIZE
                };
            }

            if (!t.GetInterfaces().Contains(typeof(IPageInfo)) || IsPagedResponseType(t))
            {
                return new NoSpecimen();
            }

            if (t.IsGenericType)
            {
                var typeArguments = t.GetGenericArguments();
                var genericType = t.GetGenericTypeDefinition();

                if (genericType == typeof(PagedResult<>) || genericType == typeof(IPagedResult<>))
                {
                    var pagedResultType = typeof(PagedResult<>).MakeGenericType(typeArguments);

                    dynamic list = context.Resolve(typeof(IImmutableList<>).MakeGenericType(typeArguments));
                    IEnumerable<Exception> errors = Enumerable.Empty<Exception>();

                    var factory = pagedResultType.GetMethod("Succeeded", BindingFlags.Public | BindingFlags.Static)!;
                    return factory.Invoke(null, [list, DEFAULT_PAGE_NUMBER, DEFAULT_PAGE_SIZE, DEFAULT_PAGE_SIZE, true])!;
                }
            }

            throw new Exception($"Cannot create a fixture value for {t.Name} which has paging information. Add support to {nameof(PageInfoSpecimenBuilder)}.");
        }
    }
}