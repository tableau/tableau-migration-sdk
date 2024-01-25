using System.Collections.Generic;
using System.Xml.Linq;

namespace Tableau.Migration.Content.Files.Xml
{
    /// <summary>
    /// Static class containing <see cref="XContainer"/> extension methods to assist with
    /// Tableau XML format's Feature Forked Subtree (FFS) mangling.
    /// </summary>
    public static class XContainerExtensions
    {
        /// <summary>
        /// Gets descendant elements at all levels that match (without FFS mangling considered) a given name.
        /// </summary>
        /// <param name="container">The container to get descendant elements for.</param>
        /// <param name="unmangledName">The unmangled/non-FFS name to search for.</param>
        /// <returns>The descendant elements with the given name.</returns>
        public static IEnumerable<XElement> GetFeatureFlaggedDescendants(this XContainer container, XName unmangledName)
        {
            foreach (var childElement in container.Descendants())
            {
                if (childElement.Name.MatchFeatureFlagName(unmangledName))
                {
                    yield return childElement;
                }
            }
        }
    }
}
