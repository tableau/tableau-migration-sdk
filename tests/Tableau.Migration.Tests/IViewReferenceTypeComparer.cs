using System;
using System.Linq;
using Tableau.Migration.Api.Rest.Models;
using Xunit;

namespace Tableau.Migration.Tests
{
    internal class IViewReferenceTypeComparer : ComparerBase<IViewReferenceType>
    {
        public static IViewReferenceTypeComparer Instance = new();

        public override int CompareItems(IViewReferenceType x, IViewReferenceType y)
        {
            Assert.NotNull(x.ContentUrl);
            Assert.NotNull(y.ContentUrl);

            // The content URL of a view is <workbook name>/<view Name>
            // As the workbook name may change during a migration, we can't rely on this
            // View renames are not supported (outside of xml transformer changes), so this will work 
            // for our testing purposes. 
            var xViewName = x.ContentUrl.Split(Constants.PathSeparator).Last();
            var yViewName = y.ContentUrl.Split(Constants.PathSeparator).Last();

            return StringComparer.Ordinal.Compare(xViewName, yViewName);
        }

    }
}
