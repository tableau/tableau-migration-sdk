
using CommunityToolkit.Diagnostics;
using Tableau.Migration.TestComponents.JsonConverters.Exceptions;

namespace Tableau.Migration.TestComponents.JsonConverters.JsonObjects
{

    public class JsonContentLocation
    {
        public string[]? PathSegments { get; set; }
        public string? PathSeparator { get; set; }
        public string? Path { get; set; }
        public string? Name { get; set; }
        public bool IsEmpty { get; set; }

        /// <summary>
        /// Throw exception if any values are still null
        /// </summary>
        public void VerifyDeseralization()
        {
            Guard.IsNotNull(PathSegments, nameof(PathSegments));
            Guard.IsNotNull(PathSeparator, nameof(PathSeparator));
            Guard.IsNotNull(Path, nameof(Path));
            Guard.IsNotNull(Name, nameof(Name));

            var expectedName = PathSegments.LastOrDefault() ?? string.Empty;
            if (!string.Equals(Name, expectedName, StringComparison.Ordinal))
                throw new MismatchException($"{nameof(Name)} should be {expectedName} but is {Name}.");

            var expectedPath = string.Join(PathSeparator, PathSegments);
            if (!string.Equals(Path, expectedPath, StringComparison.Ordinal))
                throw new MismatchException($"{nameof(Path)} should be {expectedPath} but is {Path}.");

            var expectedIsEmpty = (PathSegments.Length == 0);
            if (IsEmpty != expectedIsEmpty)
                throw new MismatchException($"{nameof(IsEmpty)} should be {expectedIsEmpty} but is {IsEmpty}.");
        }

        /// <summary>
        /// Returns the current object as a <see cref="ContentLocation"/>
        /// </summary>
        /// <returns></returns>
        public ContentLocation AsContentLocation()
        {
            VerifyDeseralization();
            var ret = new ContentLocation(PathSeparator!, PathSegments!);
            return ret;
        }
    }
}
