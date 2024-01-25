using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Tableau.Migration.Engine.Hooks.Mappings
{
    /// <summary>
    /// Default <see cref="IContentMappingBuilder"/> implementation.
    /// </summary>
    public class ContentMappingBuilder : ContentTypeHookBuilderBase, IContentMappingBuilder
    {
        /// <summary>
        /// Creates a new empty <see cref="ContentMappingBuilder"/> object.
        /// </summary>
        public ContentMappingBuilder()
        { }

        #region - Private Helper Methods -

        private IContentMappingBuilder Add(Type mappingType, Func<IServiceProvider, object> initializer)
        {
            AddFactoriesByType(mappingType, initializer);
            return this;
        }

        #endregion

        #region - IContentMappingBuilder Implementation -

        /// <inheritdoc />
        public virtual IContentMappingBuilder Clear()
        {
            ClearFactories();
            return this;
        }

        /// <inheritdoc />
        public virtual IContentMappingBuilder Add<TContent>(IContentMapping<TContent> mapping)
            where TContent : IContentReference
            => Add(typeof(IContentMapping<TContent>), s => mapping);

        /// <inheritdoc />
        public virtual IContentMappingBuilder Add<TMapping, TContent>(Func<IServiceProvider, TMapping>? mappingFactory = null)
            where TMapping : IContentMapping<TContent>
            where TContent : IContentReference
        {
            mappingFactory ??= services => services.GetRequiredService<TMapping>();
            return Add(typeof(IContentMapping<TContent>), s => mappingFactory(s));
        }

        /// <inheritdoc />
        public virtual IContentMappingBuilder Add<TContent>(Func<ContentMappingContext<TContent>, CancellationToken, Task<ContentMappingContext<TContent>?>> callback)
            where TContent : IContentReference
            => Add(typeof(IContentMapping<TContent>),
                   s => new CallbackHookWrapper<IContentMapping<TContent>, ContentMappingContext<TContent>>(callback));

        /// <inheritdoc />
        public IContentMappingBuilder Add<TContent>(Func<ContentMappingContext<TContent>, ContentMappingContext<TContent>?> callback)
            where TContent : IContentReference
            => Add<TContent>(
                (ctx, cancel) => Task.FromResult(
                    callback(ctx)));

        #endregion
    }
}
