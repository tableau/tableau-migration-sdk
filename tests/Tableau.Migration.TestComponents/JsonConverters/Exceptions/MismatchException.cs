namespace Tableau.Migration.TestComponents.JsonConverters.Exceptions
{
    public class MismatchException : Exception
    {
        /// <summary>
        /// The exception that is thrown when a <see cref="IMigrationManifest"/> that was serialized and then deserialized but did not match the initial Manifest.
        /// </summary>
        /// <remarks>This means that the either the serializer or deserializer has a bug.</remarks>
        public MismatchException()
        {
        }

        /// <summary>
        /// The exception that is thrown when a <see cref="IMigrationManifest"/> that was serialized and then deserialized but did not match the initial Manifest.
        /// </summary>
        /// <remarks>This means that the either the serializer or deserializer has a bug.</remarks>
        public MismatchException(string message) : base(message)
        {
        }

        /// <summary>
        /// The exception that is thrown when a <see cref="IMigrationManifest"/> that was serialized and then deserialized but did not match the initial Manifest.
        /// </summary>
        /// <remarks>This means that the either the serializer or deserializer has a bug.</remarks>
        public MismatchException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}