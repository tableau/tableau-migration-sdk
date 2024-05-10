//
//  Copyright (c) 2024, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the "License") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using System;
using System.Xml.Linq;

namespace Tableau.Migration.Content.Files.Xml
{
    /// <summary>
    /// Structure representing a XML <see cref="XName"/> that tracks and demangles any Feature Forked Subtree (FFS) information.
    /// </summary>
    public struct XFeatureFlagName
    {
        internal const string FFS_PREFIX = "_.fcp.";

        internal const string FFS_NODE_NAME_DELIMITER = "...";

        internal const string FFS_PREFIX_PART_DELIMITER = ".";

        /// <summary>
        /// Gets whether or not the name has a feature flag component. 
        /// Will be false for simple/unmangled names.
        /// </summary>
        public bool HasFeatureFlag { get; }

        /// <summary>
        /// Gets the name of the feature flag used in FFS mangling. 
        /// Will be an empty string for simple/unmangled names.
        /// </summary>
        public string FeatureFlagName { get; }

        /// <summary>
        /// Gets whether the feature flag is enabled or disabled for this element/attribute.
        /// Will be false for simple/unmangled names.
        /// </summary>
        public bool FeatureFlagEnabled { get; }

        /// <summary>
        /// Gets the unmangled element/attribute name without FFS prefix information.
        /// Will equal <see cref="FullName"/> for simple/unmangled names.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the full <see cref="XName"/>, including with any FFS mangling still intact.
        /// </summary>
        public XName FullName { get; }

        private XFeatureFlagName(XName xmlName)
        {
            FullName = xmlName;

            HasFeatureFlag = FeatureFlagEnabled = false;
            Name = FeatureFlagName = string.Empty;

            var mangledName = FullName.LocalName;

            if (string.IsNullOrEmpty(mangledName) || !mangledName.StartsWith(FFS_PREFIX, StringComparison.Ordinal))
            {
                Name = mangledName;
            }
            else
            {
                HasFeatureFlag = true;

                var nodeNameParts = mangledName.Split(new[] { FFS_NODE_NAME_DELIMITER }, StringSplitOptions.RemoveEmptyEntries);
                if (nodeNameParts.Length > 0)
                {
                    var prefixParts = nodeNameParts[0].Split(new[] { FFS_PREFIX_PART_DELIMITER }, StringSplitOptions.RemoveEmptyEntries);

                    if (prefixParts.Length > 2)
                    {
                        FeatureFlagName = prefixParts[2];

                        if (prefixParts.Length > 3)
                        {
                            FeatureFlagEnabled = bool.Parse(prefixParts[3].ToLower());
                        }
                    }
                }

                if (nodeNameParts.Length > 1)
                {
                    Name = nodeNameParts[1];
                }
            }
        }

        /// <summary>
        /// Creates a <see cref="XFeatureFlagName"/> from an XML <see cref="XName"/>, which can optionally be mangled with FFS information.
        /// </summary>
        /// <param name="xmlName">The name to parse.</param>
        /// <returns>The created <see cref="XFeatureFlagName"/>.</returns>
        public static XFeatureFlagName Parse(XName xmlName)
        {
            return new XFeatureFlagName(xmlName);
        }

        /// <inheritdoc/>
        public readonly override bool Equals(object? obj)
        {
            if (obj is not null && obj is XFeatureFlagName otherName)
            {
                return FullName == otherName.FullName;
            }

            return false;
        }

        /// <inheritdoc/>
        public readonly override int GetHashCode()
        {
            return FullName.GetHashCode();
        }

        /// <inheritdoc/>
        public static bool operator ==(XFeatureFlagName left, XFeatureFlagName right)
        {
            return left.Equals(right);
        }

        /// <inheritdoc/>
        public static bool operator !=(XFeatureFlagName left, XFeatureFlagName right)
        {
            return !(left == right);
        }
    }
}
