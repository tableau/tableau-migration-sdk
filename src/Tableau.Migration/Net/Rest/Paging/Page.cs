namespace Tableau.Migration.Net.Rest.Paging
{
    /// <summary>
    /// <para>
    /// Class a REST API page
    /// </para>
    /// <para>
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_concepts_paging.htm">Tableau API Reference</see> for more details.
    /// </para>
    /// </summary>
    /// <param name="PageNumber">Gets the page size.</param>
    /// <param name="PageSize">Gets the page number.</param>
    public readonly record struct Page(int PageNumber, int PageSize)
    { }
}
