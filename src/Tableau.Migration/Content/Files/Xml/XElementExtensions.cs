using System.Collections.Generic;
using System.Xml.Linq;

namespace Tableau.Migration.Content.Files.Xml
{
    /// <summary>
    /// Static class containing <see cref="XElement"/> extension methods to assist with
    /// Tableau XML format's Feature Forked Subtree (FFS) mangling.
    /// </summary>
    public static class XElementExtensions
    {
        /// <summary>
        /// Gets all attributes that match (without FFS mangling considered) a given name.
        /// </summary>
        /// <param name="el">The element to get attributes for.</param>
        /// <param name="unmangledName">The unmangled/non-FFS name to search for.</param>
        /// <returns>The attributes with the given name.</returns>
        public static IEnumerable<XAttribute> GetFeatureFlaggedAttributes(this XElement el, XName unmangledName)
        {
            foreach (var atr in el.Attributes())
            {
                if (atr.Name.MatchFeatureFlagName(unmangledName))
                {
                    yield return atr;
                }
            }
        }
    }
}
