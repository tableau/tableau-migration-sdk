
using CommunityToolkit.Diagnostics;
using Tableau.Migration.Content;

namespace Tableau.Migration.TestComponents.JsonConverters.JsonObjects
{

    public class JsonContentReference
    {
        public string? Id { get; set; }
        public string? ContentUrl { get; set; }
        public JsonContentLocation? Location { get; set; }
        public string? Name { get; set; }

        /// <summary>
        /// Throw exception if any values are still null
        /// </summary>
        public void VerifyDeseralization()
        {
            Guard.IsNotNull(Id, nameof(Id));
            Guard.IsNotNull(ContentUrl, nameof(ContentUrl));
            Guard.IsNotNull(Location, nameof(Location));
            Guard.IsNotNull(Name, nameof(Name));
        }

        /// <summary>
        /// Returns the current item as a <see cref="ContentReferenceStub"/>
        /// </summary>
        /// <returns></returns>
        public ContentReferenceStub AsContentReferenceStub()
        {
            VerifyDeseralization();

            var ret = new ContentReferenceStub(
                Guid.Parse(Id!),
                ContentUrl!,
                Location!.AsContentLocation(),
                Name!);

            return ret;
        }
    }
}