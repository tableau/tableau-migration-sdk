using System;
using Tableau.Migration.Api.Rest.Models;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models
{
    internal class ITagTypeComparer : ComparerBase<ITagType>
    {
        public static readonly ITagTypeComparer Instance = new();

        public override int CompareItems(ITagType x, ITagType y) => StringComparer.Ordinal.Compare(x.Label, y.Label);
    }
}
