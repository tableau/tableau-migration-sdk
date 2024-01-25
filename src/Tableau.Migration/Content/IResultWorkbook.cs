namespace Tableau.Migration.Content
{
    /// <summary>
    /// Intreface for the publish result of an <see cref="IWorkbook"/>.
    /// </summary>
    public interface IResultWorkbook : IWorkbook, IWithViews, IChildPermissionsContent
    { }
}
