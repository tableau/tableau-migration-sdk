namespace Tableau.Migration.Config
{
    /// <summary>
    /// Interface to fetch the current <see cref="MigrationSdkOptions"/>.
    /// </summary>
    public interface IConfigReader
    {
        /// <summary>
        /// Get the current <see cref="MigrationSdkOptions"/>.
        /// </summary>
        /// <returns></returns>
        MigrationSdkOptions Get();
    }
}