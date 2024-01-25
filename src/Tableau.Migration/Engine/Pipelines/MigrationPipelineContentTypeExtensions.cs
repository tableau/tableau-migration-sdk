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
