namespace Tableau.Migration
{
    /// <summary>
    /// Enumeration of the various ways a migration can reach completion.
    /// </summary>
    public enum MigrationCompletionStatus
    {
        /// <summary>
        /// The migration reached completion normally.
        /// </summary>
        Completed = 0,

        /// <summary>
        /// The migration was canceled before completion.
        /// </summary>
        Canceled,

        /// <summary>
        /// The migration had a fatal error that interrupted completion.
        /// </summary>
        FatalError
    }
}
