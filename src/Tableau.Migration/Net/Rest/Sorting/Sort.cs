using System.ComponentModel;

namespace Tableau.Migration.Net.Rest.Sorting
{
    /// <summary>
    /// <para>
    /// Class representing a REST API sort
    /// </para>
    /// <para>
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_concepts_filtering_and_sorting.htm#sorting">Tableau API Reference</see> for more details.
    /// </para>
    /// </summary>
    public class Sort
    {
        /// <summary>
        /// Gets the field to sort on.
        /// </summary>
        public string Field { get; }

        /// <summary>
        /// Gets the direction to sort.
        /// </summary>
        public ListSortDirection Direction { get; }

        /// <summary>
        /// Gets the sort's expression for use in query strings.
        /// </summary>
        public string Expression { get; }

        /// <summary>
        /// Creates a new <see cref="Sort"/> instance.
        /// </summary>
        /// <param name="field">The field to sort on.</param>
        /// <param name="direction">The direction to sort.</param>
        public Sort(string field, ListSortDirection direction)
        {
            Field = Guard.AgainstNullEmptyOrWhiteSpace(field, nameof(field));
            Direction = direction;
            Expression = $"{Field}:{Direction.GetQueryStringValue()}";
        }

        /// <summary>
        /// Creates a new <see cref="Sort"/> instance.
        /// </summary>
        /// <param name="field">The field to sort on.</param>
        /// <param name="ascending">Try to sort ascending, false to sort descending.</param>
        public Sort(string field, bool ascending)
            : this(field, ascending ? ListSortDirection.Ascending : ListSortDirection.Descending)
        { }
    }
}
