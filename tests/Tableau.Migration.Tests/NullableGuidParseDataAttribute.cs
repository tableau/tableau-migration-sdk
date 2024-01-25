using System;

namespace Tableau.Migration.Tests
{
    public class NullableGuidParseDataAttribute : NullEmptyWhiteSpaceDataAttribute
    {
        public NullableGuidParseDataAttribute()
            : base(Guid.NewGuid().ToString())
        { }

        protected override object?[] CreateArguments(string? value)
            => Guid.TryParse(value, out var guidValue) 
                ? new object?[] { value, guidValue }
                : new object?[] { value, null };
    }
}
