namespace Tableau.Migration.Config
{
    /// <summary>
    /// Network options related to Tableau connections.
    /// </summary>
    public class NetworkOptions
    {
        /// <summary>
        /// Defaults for network settings.
        /// </summary>
        public static class Defaults
        {
            /// <summary>
            /// The default file chunk size in Kilobytes.
            /// </summary>
            public const int FILE_CHUNK_SIZE_KB = 512;

            /// <summary>
            /// The default Network Headers Logging Flag - Disabled as Default.
            /// </summary>
            public const bool LOG_HEADERS_ENABLED = false;

            /// <summary>
            /// The default Network Content Logging Flag - Disabled as Default.
            /// </summary>
            public const bool LOG_CONTENT_ENABLED = false;

            /// <summary>
            /// The default Network Binary Content Logging Flag - Disabled as Default.
            /// </summary>
            public const bool LOG_BINARY_CONTENT_ENABLED = false;

            /// <summary>
            /// The default Network Exceptions Logging Flag - Disabled as Default.
            /// </summary>
            public const bool LOG_EXCEPTIONS_ENABLED = false;
        }

        /// <summary>
        /// Maximum file chunk size in Kilobytes.
        /// </summary>
        public int FileChunkSizeKB
        {
            get => _fileChunkSizeKB ?? Defaults.FILE_CHUNK_SIZE_KB;
            set => _fileChunkSizeKB = value;
        }
        private int? _fileChunkSizeKB;

        /// <summary>
        /// Indicates whether the SDK logs request/response headers. The default value is disabled.
        /// </summary>
        public bool HeadersLoggingEnabled
        {
            get => _headersLoggingEnabled ?? Defaults.LOG_HEADERS_ENABLED;
            set => _headersLoggingEnabled = value;
        }
        private bool? _headersLoggingEnabled;

        /// <summary>
        /// Indicates whether the SDK logs request/response content. The default value is disabled.
        /// </summary>
        public bool ContentLoggingEnabled
        {
            get => _contentLoggingEnabled ?? Defaults.LOG_CONTENT_ENABLED;
            set => _contentLoggingEnabled = value;
        }
        private bool? _contentLoggingEnabled;

        /// <summary>
        /// Indicates whether the SDK logs request/response binary (not textual) content. The default value is disabled.
        /// </summary>
        public bool BinaryContentLoggingEnabled
        {
            get => _binaryContentLoggingEnabled ?? Defaults.LOG_BINARY_CONTENT_ENABLED;
            set => _binaryContentLoggingEnabled = value;
        }
        private bool? _binaryContentLoggingEnabled;

        /// <summary>
        /// Indicates whether the SDK logs network exceptions. The default value is disabled.
        /// </summary>
        public bool ExceptionsLoggingEnabled
        {
            get => _exceptionsLoggingEnabled ?? Defaults.LOG_EXCEPTIONS_ENABLED;
            set => _exceptionsLoggingEnabled = value;
        }
        private bool? _exceptionsLoggingEnabled;

        /// <summary>
        /// Resilience options related to Tableau connections. 
        /// This configuration adds a transient-fault-handling layer for all communication to Tableau.
        /// </summary>
        public ResilienceOptions Resilience { get; set; } = new();
    }
}