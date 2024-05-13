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
using System.Linq;

namespace Tableau.Migration.Engine.Pipelines
{
    internal static class MigrationPipelineContentTypeExtensions
    {
        public static IImmutableList<Type[]> WithContentTypeInterface(
            this IEnumerable<MigrationPipelineContentType> contentTypes,
            Type @interface)
            => contentTypes.FindTypes(t => t.GetContentTypeForInterface(@interface));

        public static IImmutableList<Type[]> WithContentTypeInterface<TInterface>(this IEnumerable<MigrationPipelineContentType> contentTypes)
            => contentTypes.WithContentTypeInterface(typeof(TInterface));

        public static IImmutableList<Type[]> WithPublishTypeInterface(
            this IEnumerable<MigrationPipelineContentType> contentTypes,
            Type @interface)
            => contentTypes.FindTypes(t => t.GetPublishTypeForInterface(@interface));

        public static IImmutableList<Type[]> WithPublishTypeInterface<TInterface>(this IEnumerable<MigrationPipelineContentType> contentTypes)
            => contentTypes.WithPublishTypeInterface(typeof(TInterface));

        public static IImmutableList<Type[]> WithPostPublishTypeInterface(
            this IEnumerable<MigrationPipelineContentType> contentTypes,
            Type @interface)
            => contentTypes.FindTypes(t => t.GetPostPublishTypesForInterface(@interface));

        public static IImmutableList<Type[]> WithPostPublishTypeInterface<TInterface>(this IEnumerable<MigrationPipelineContentType> contentTypes)
            => contentTypes.WithPostPublishTypeInterface(typeof(TInterface));

        private static IImmutableList<Type[]> FindTypes(
            this IEnumerable<MigrationPipelineContentType> contentTypes,
            Func<MigrationPipelineContentType, Type[]?> find)
            => contentTypes
                .Select(find)
                .Where(t => !t.IsNullOrEmpty())
                .ToImmutableArray();
    }
}
