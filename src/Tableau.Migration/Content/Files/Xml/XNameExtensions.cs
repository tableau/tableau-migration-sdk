using System;
using System.Xml.Linq;

namespace Tableau.Migration.Content.Files.Xml
{
    /// <summary>
    /// Static class containing <see cref="XName"/> extension methods to assist with
    /// Tableau XML format's Feature Forked Subtree (FFS) mangling.
    /// </summary>
    public static class XNameExtensions
    {
        private const StringComparison TABLEAU_XML_COMPARISON = StringComparison.OrdinalIgnoreCase;

        /// <summary>
        /// Matches an XML name to another, considering 
        /// 1) namespace and 
        /// 2) complete local name matches or suffix matches to the whole <paramref name="matchName"/> if <paramref name="xmlName"/> has an FFS prefix.
        /// All comparisons are case insensitive.
        /// </summary>
        /// <param name="xmlName">The (possibly FFS prefixed) name to test.</param>
        /// <param name="matchName">The XML name to match to - should not contain any FFS prefix.</param>
        /// <returns>True if the name matches given FFS considerations, otherwise false.</returns>
        public static bool MatchFeatureFlagName(this XName xmlName, XName matchName)
        {
            if (matchName.LocalName.StartsWith(XFeatureFlagName.FFS_PREFIX, TABLEAU_XML_COMPARISON))
                throw new ArgumentException("Feature flag match name should not have any FFS mangling.", nameof(matchName));

            if (!string.Equals(xmlName.NamespaceName, matchName.NamespaceName, TABLEAU_XML_COMPARISON))
            {
                return false;
            }

            var localName = matchName.LocalName;

            //Match direct equality first.
            if (string.Equals(xmlName.LocalName, localName, TABLEAU_XML_COMPARISON))
            {
                return true;
            }

            //Match FFS suffix 
            if (xmlName.LocalName.StartsWith(XFeatureFlagName.FFS_PREFIX, TABLEAU_XML_COMPARISON))
            {
                var matchFfsSuffix = XFeatureFlagName.FFS_NODE_NAME_DELIMITER + localName;
                if (xmlName.LocalName.EndsWith(matchFfsSuffix, TABLEAU_XML_COMPARISON))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
