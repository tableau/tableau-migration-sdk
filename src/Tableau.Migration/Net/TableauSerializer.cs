namespace Tableau.Migration.Net
{
    /// <summary>
    /// Class for serializing Tableau Server request and response content.
    /// </summary>
    internal sealed class TableauSerializer : ITableauSerializer
    {
        internal static readonly TableauSerializer Instance = new();
    }
}
