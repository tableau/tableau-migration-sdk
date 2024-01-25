using System;
using Tableau.Migration.Api.Rest.Models;

namespace Tableau.Migration.Tests
{
    internal class ITagTypeComparer : ComparerBase<ITagType>
    {
        public static ITagTypeComparer Instance = new();

        public override int CompareItems(ITagType x, ITagType y)
            => StringComparer.Ordinal.Compare(x.Label, y.Label);
    }
}
