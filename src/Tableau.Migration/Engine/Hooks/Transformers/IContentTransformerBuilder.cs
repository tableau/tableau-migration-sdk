﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Engine.Hooks.Transformers
{
    /// <summary>
    /// Interface with methods to build <see cref="IContentTransformer{TContent}"/>.
    /// </summary>
    public interface IContentTransformerBuilder : IContentTypeHookBuilder
    {
        /// <summary>
        /// Removes all currently registered transformers.
        /// </summary>
        /// <returns>The same transformers builder object for fluent API calls.</returns>
        IContentTransformerBuilder Clear();

        /// <summary>
        /// Adds a factory to resolve the transformer type.
        /// </summary>
        /// <param name="genericTransformerType">The generic type definition for the transformer to execute.</param>
        /// <param name="contentTypes">The content types used to construct the transformer types.</param>
        /// <returns>The same transformer builder object for fluent API calls.</returns>
        IContentTransformerBuilder Add(Type genericTransformerType, IEnumerable<Type[]> contentTypes);

        /// <summary>
        /// Adds an object to be resolved when you build a transformer for the content type.
        /// </summary>
        /// <typeparam name="TPublish">The publishable content type.</typeparam>
        /// <param name="transformer">The transformer to execute.</param>
        /// <returns>The same transformer builder object for fluent API calls.</returns>
        IContentTransformerBuilder Add<TPublish>(IContentTransformer<TPublish> transformer);

        /// <summary>
        /// Adds a factory to resolve the transformer type.
        /// </summary>
        /// <typeparam name="TTransformer">The transformer type.</typeparam>
        /// <typeparam name="TPublish">The publishable content type.</typeparam>
        /// <param name="transformerFactory">An initializer function to create the object from, potentially from the migration-scoped dependency injection container.</param>
        /// <returns>The same transformer builder object for fluent API calls.</returns>
        IContentTransformerBuilder Add<TTransformer, TPublish>(Func<IServiceProvider, TTransformer>? transformerFactory = null)
            where TTransformer : IContentTransformer<TPublish>;

        /// <summary>
        /// Adds a callback to be executed on the transformer for the content type.
        /// </summary>
        /// <typeparam name="TPublish">The publishable content type.</typeparam>
        /// <param name="callback">A callback to call for the transformer.</param>
        /// <returns>The same transformer builder object for fluent API calls.</returns>
        IContentTransformerBuilder Add<TPublish>(Func<TPublish, CancellationToken, Task<TPublish?>> callback);

        /// <summary>
        /// Adds a callback to be executed on the transformer for the content type.
        /// </summary>
        /// <typeparam name="TPublish">The publishable content type.</typeparam>
        /// <param name="callback">A synchronously callback to call for the transformer.</param>
        /// <returns>The same transformer builder object for fluent API calls.</returns>
        IContentTransformerBuilder Add<TPublish>(Func<TPublish, TPublish?> callback);

        /// <summary>
        /// Builds an immutable collection from the currently added transformers.
        /// </summary>
        /// <returns>The created collection.</returns>
        IMigrationHookFactoryCollection Build();
    }
}