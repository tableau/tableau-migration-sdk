using System;
using System.Collections.Generic;

namespace Tableau.Migration.Engine.Hooks
{
    /// <summary>
    /// Interface for a builder of hooks that use a common hook type
    /// that differ by a content type.
    /// </summary>
    public interface IContentTypeHookBuilder
    {
        /// <summary>
        /// Gets the currently registered hook factories by their content types.
        /// </summary>
        /// <returns>The hook factories by their content types.</returns>
        IEnumerable<KeyValuePair<Type, IEnumerable<IMigrationHookFactory>>> ByContentType();
    }
}
