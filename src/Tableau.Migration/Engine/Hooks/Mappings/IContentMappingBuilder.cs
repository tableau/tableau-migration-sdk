using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Engine.Hooks.Mappings
{
    /// <summary>
    /// Interface with methods to build <see cref="IContentMapping{TContent}"/>.
    /// </summary>
    public interface IContentMappingBuilder : IContentTypeHookBuilder
    {
        /// <summary>
        /// Removes all currently registered mappings.
        /// </summary>
        /// <returns>The same mapping builder object for fluent API calls.</returns>
        IContentMappingBuilder Clear();

        /// <summary>
        /// Adds an object to be resolved when you build a mapping for the content type.
        /// </summary>
        /// <typeparam name="TContent">The content type.</typeparam>
        /// <param name="mapping">The mapping to execute.</param>
        /// <returns>The same mapping builder object for fluent API calls.</returns>
        IContentMappingBuilder Add<TContent>(
            IContentMapping<TContent> mapping)
            where TContent : IContentReference;

        /// <summary>
        /// Adds a factory to resolve the mapping type.
        /// </summary>
        /// <typeparam name="TMapping">The mapping type.</typeparam>
        /// <typeparam name="TContent">The content type.</typeparam>
        /// <param name="mappingFactory">An initializer function to create the object from, potentially from the migration-scoped dependency injection container.</param>
        /// <returns>The same mapping builder object for fluent API calls.</returns>
        IContentMappingBuilder Add<TMapping, TContent>(
            Func<IServiceProvider, TMapping>? mappingFactory = null)
            where TMapping : IContentMapping<TContent>
            where TContent : IContentReference;

        /// <summary>
        /// Adds a callback to be executed on the mapping for the content type.
        /// </summary>
        /// <typeparam name="TContent">The content type.</typeparam>
        /// <param name="callback">A callback to call for the mapping.</param>
        /// <returns>The same mapping builder object for fluent API calls.</returns>
        IContentMappingBuilder Add<TContent>(
            Func<ContentMappingContext<TContent>, CancellationToken, Task<ContentMappingContext<TContent>?>> callback)
            where TContent : IContentReference;

        /// <summary>
        /// Adds a callback to be executed on the mapping for the content type.
        /// </summary>
        /// <typeparam name="TContent">The content type.</typeparam>
        /// <param name="callback">A synchronously callback to call for the mapping.</param>
        /// <returns>The same mapping builder object for fluent API calls.</returns>
        IContentMappingBuilder Add<TContent>(
            Func<ContentMappingContext<TContent>, ContentMappingContext<TContent>?> callback)
            where TContent : IContentReference;

        /// <summary>
        /// Builds an immutable collection from the currently added mappings.
        /// </summary>
        /// <returns>The created collection.</returns>
        IMigrationHookFactoryCollection Build();
    }
}