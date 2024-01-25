using System;
using System.Xml.Serialization;
using Tableau.Migration.Content;

namespace Tableau.Migration.Api.Rest.Models.Responses
{
    /// <summary>    
    /// Class representing an add-tag request.
    /// </summary>
    [XmlType(XmlTypeName)]
    public class AddTagsResponse : TableauServerListResponse<AddTagsResponse.TagType>
    {
        /// <summary>
        /// The default parameterless constructor.
        /// </summary>
        public AddTagsResponse()
        { }

        /// <summary>
        /// The array of tags to add.
        /// </summary>
        [XmlArray("tags")]
        [XmlArrayItem("tag")]
        public override TagType[] Items { get; set; } = Array.Empty<TagType>();

        /// <inheritdoc/>
        public class TagType : ITagType
        {
            /// <inheritdoc/>
            [XmlAttribute("label")]
            public string? Label { get; set; }

            /// <summary>
            /// The default parameterless constructor.
            /// </summary>            
            public TagType()
            { }

            /// <summary>
            /// Constructor to build from <see cref="ITagType"/>
            /// </summary>
            /// <param name="tag">The <see cref="ITagType"/> object.</param>
            public TagType(ITagType tag)
            {
                Label = tag.Label;
            }

            /// <summary>
            /// Constructor to build from <see cref="ITag"/>
            /// </summary>
            /// <param name="tag">The <see cref="ITag"/> object.</param>
            public TagType(ITag tag)
            {
                Label = tag.Label;
            }
        }
    }
}
