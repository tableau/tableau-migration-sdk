using System.ComponentModel;

namespace Tableau.Migration.Net.Rest
{
    internal static class ListSortDirectionExtensions
    {
        public static string GetQueryStringValue(this ListSortDirection direction) =>
            direction == ListSortDirection.Ascending ? "asc" : "desc";
    }
}
