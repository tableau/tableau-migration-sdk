using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Tableau.Migration.Content;

namespace Tableau.Migration.Api.Rest.Models.Requests
{
    /// <summary>    
    /// Class representing an add-tag request.
    /// </summary>
    [XmlType(XmlTypeName)]
    public class AddTagsRequest : TableauServerRequest
    {
        /// <summary>
        /// Create a new <see cref="AddTagsRequest"/> object.
        /// </summary>
        public AddTagsRequest()
        { }

        /// <summary>
        /// Create a new <see cref="AddTagsRequest"/> object.
        /// </summary>
        /// <param name="tags">The tags to populate the request with.</param>
        public AddTagsRequest(IEnumerable<ITag> tags)
        {
            Tags = tags.Select(tag => new TagType(tag)).ToArray() ?? Array.Empty<TagType>();
        }

        /// <summary>
        /// The array of tags to add.
        /// </summary>
        [XmlArray("tags")]
        [XmlArrayItem("tag")]
        public TagType[]? Tags { get; set; } = Array.Empty<TagType>();

        /// <inheritdoc/>
        public class TagType : ITagType
        {
            /// <inheritdoc/>
            [XmlAttribute("label")]
            public string? QuotedLabel { get; set; }

            /// <inheritdoc/>
            [XmlIgnore]
            public string? Label
            {
                get
                {
                    if (QuotedLabel.IsNullOrEmpty())
                    {
                        return QuotedLabel;
                    }
                    else
                    {
                        var label = QuotedLabel;
                        if (label.StartsWith("\""))
                        {
                            label = label.Substring(1);
                        }

                        if (label.EndsWith("\""))
                        {
                            label = label.Substring(0, label.Length - 1);
                        }

                        return label;
                    }
                }
                set
                {
                    if (value.IsNullOrEmpty())
                    {
                        QuotedLabel = value;
                    }
                    else
                    {
                        QuotedLabel = $"\"{value}\"";
                    }
                }
            }

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
