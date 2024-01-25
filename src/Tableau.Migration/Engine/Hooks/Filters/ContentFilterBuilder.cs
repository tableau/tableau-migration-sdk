using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Tableau.Migration.Engine.Hooks.Filters
{
    /// <summary>
    /// Default <see cref="IContentFilterBuilder"/> implementation.
    /// </summary>
    public class ContentFilterBuilder : ContentTypeHookBuilderBase, IContentFilterBuilder
    {
        /// <summary>
        /// Creates a new empty <see cref="MigrationHookBuilder"/> object.
        /// </summary>
        public ContentFilterBuilder()
        { }

        #region - Private Helper Methods -

        private IContentFilterBuilder Add(Type filterType, Func<IServiceProvider, object> initializer)
        {
            AddFactoriesByType(filterType, initializer);
            return this;
        }

        #endregion

        #region - IContentFilterBuilder Implementation -

        /// <inheritdoc />
        public virtual IContentFilterBuilder Clear()
        {
            ClearFactories();
            return this;
        }

        /// <inheritdoc />
        public virtual IContentFilterBuilder Add(Type genericTransformerType, IEnumerable<Type[]> contentTypes)
        {
            if (!genericTransformerType.IsGenericTypeDefinition)
                throw new ArgumentException($"Type {genericTransformerType.FullName} is not a generic type definition.");

            foreach (var contentType in contentTypes)
            {
                var constructedType = genericTransformerType.MakeGenericType(contentType);

                object transformerFactory(IServiceProvider services)
                {
                    return services.GetRequiredService(constructedType);
                }

                Add(constructedType, transformerFactory);
            }

            return this;
        }

        /// <inheritdoc />
        public virtual IContentFilterBuilder Add<TContent>(IContentFilter<TContent> filter)
            where TContent : IContentReference
            => Add(typeof(IContentFilter<TContent>), s => filter);

        /// <inheritdoc />
        public virtual IContentFilterBuilder Add<TFilter, TContent>(Func<IServiceProvider, TFilter>? filterFactory = null)
            where TFilter : IContentFilter<TContent>
            where TContent : IContentReference
        {
            filterFactory ??= services => services.GetRequiredService<TFilter>();
            return Add(typeof(IContentFilter<TContent>), s => filterFactory(s));
        }

        /// <inheritdoc />
        public virtual IContentFilterBuilder Add<TContent>(Func<IEnumerable<ContentMigrationItem<TContent>>, CancellationToken, Task<IEnumerable<ContentMigrationItem<TContent>>?>> callback)
            where TContent : IContentReference
            => Add(typeof(IContentFilter<TContent>),
                   s => new CallbackHookWrapper<IContentFilter<TContent>, IEnumerable<ContentMigrationItem<TContent>>>(callback));

        /// <inheritdoc />
        public IContentFilterBuilder Add<TContent>(Func<IEnumerable<ContentMigrationItem<TContent>>, IEnumerable<ContentMigrationItem<TContent>>?> callback)
            where TContent : IContentReference
            => Add<TContent>(
                (ctx, cancel) => Task.FromResult(
                    callback(ctx)));

        #endregion
    }
}
