namespace Tableau.Migration.Config
{
    /// <summary>
    /// Configuration options for the <see cref="Migration"/> SDK.
    /// </summary>
    public class MigrationSdkOptions
    {
        /// <summary>
        /// Defaults for migration options.
        /// </summary>
        public static class Defaults
        {
            /// <summary>
            /// The default migration batch size. 
            /// </summary>
            public const int BATCH_SIZE = 100;

            /// <summary>
            /// The default number of items to migrate in parallel.
            /// </summary>
            public const int MIGRATION_PARALLELISM = 10;
        }

        /// <summary>
        /// Gets or sets the migration batch size.
        /// </summary>
        public int BatchSize
        {
            get => _batchSize ?? Defaults.BATCH_SIZE;
            set => _batchSize = value;
        }
        private int? _batchSize;

        /// <summary>
        /// Gets or sets the number of items to migrate in parallel.
        /// </summary>
        public int MigrationParallelism
        {
            get => _migrationParallelism ?? Defaults.MIGRATION_PARALLELISM;
            set => _migrationParallelism = value;
        }
        private int? _migrationParallelism;

        /// <summary>
        /// Gets or sets options related to file storage.
        /// </summary>
        public FileOptions Files { get; set; } = new();

        /// <summary>
        /// Gets or sets options related to Tableau connections.
        /// </summary>
        public NetworkOptions Network { get; set; } = new();

        /// <summary>
        /// Default project permissions content types.
        /// </summary>
        public DefaultPermissionsContentTypeOptions DefaultPermissionsContentTypes { get; set; } = new();

        /// <summary>
        /// Gets or sets options related to jobs.
        /// </summary>
        public JobOptions Jobs { get; set; } = new();
    }
}