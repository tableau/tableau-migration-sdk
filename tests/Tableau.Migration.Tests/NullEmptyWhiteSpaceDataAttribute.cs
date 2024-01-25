using System;
using System.Collections.Immutable;
using System.Linq;

namespace Tableau.Migration.Tests
{
    public class NullEmptyWhiteSpaceDataAttribute : ValuesAttribute<string?>
    {
        private static readonly IImmutableList<string?> _values = ImmutableArray.Create(
            null,
            String.Empty,
            " ",
            "\t"
        );

        public NullEmptyWhiteSpaceDataAttribute()
            : base(_values)
        { }

        protected NullEmptyWhiteSpaceDataAttribute(params string?[] extraValues)
            : base(_values.Concat(extraValues))
        { }
    }
}
