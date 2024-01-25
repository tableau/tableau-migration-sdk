namespace Tableau.Migration
{
    /// <summary>
    /// Interface for a result of a migration.
    /// </summary>
    /// <param name="Status">How the migration reached completion.</param>
    /// <param name="Manifest">Gets the <see cref="IMigrationManifest"/> the migration produced.</param>
    public record struct MigrationResult(MigrationCompletionStatus Status, IMigrationManifest Manifest)
    { }
}
