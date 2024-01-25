using System;
using System.Collections.Generic;
using System.Linq;

namespace Tableau.Migration.Engine.Hooks
{
    /// <summary>
    /// Abstract <see cref="IMigrationHookBuilder"/>
    /// that implements <see cref="IContentTypeHookBuilder"/>.
    /// </summary>
    public abstract class ContentTypeHookBuilderBase
        : MigrationHookBuilderBase, IContentTypeHookBuilder
    {
        /// <inheritdoc />
        public IEnumerable<KeyValuePair<Type, IEnumerable<IMigrationHookFactory>>> ByContentType()
        {
            static Type GetContentType(Type hookType)
                => hookType.GenericTypeArguments.Single();

            foreach (var item in GetFactories())
            {
                yield return new(GetContentType(item.Key), item.Value);
            }
        }
    }
}
