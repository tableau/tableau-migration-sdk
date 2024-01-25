using System;
using Tableau.Migration.Net.Rest;

namespace Tableau.Migration.Api.Rest
{
    /// <summary>
    /// Class representing a URI builder for content items in a general way.
    /// Examples include permissions and tags.
    /// </summary>
    internal class ContentItemUriBuilderBase
    {
        public string Prefix { get; }

        public string Suffix { get; }

        public ContentItemUriBuilderBase(string prefix, string suffix)
        {
            Prefix = Guard.AgainstNullEmptyOrWhiteSpace(prefix, nameof(prefix));
            Suffix = Guard.AgainstNullEmptyOrWhiteSpace(suffix, nameof(suffix));
        }

        public virtual string BuildUri(Guid contentItemId)
            => $"{Prefix}/{contentItemId.ToUrlSegment()}/{Suffix}";
    }
}
