using System;
using System.Xml.Serialization;

namespace Tableau.Migration.Api.Rest.Models.Requests
{
    /// <summary>
    /// <para>
    /// Class representing a get labels request.
    /// </para>
    /// <para>
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_metadata.htm#get_labels_on_assets">Tableau API Reference</see> 
    /// for documentation.
    /// </para>
    /// </summary>
    [XmlType(XmlTypeName)]
    public class GetLabelsRequest : TableauServerRequest
    {
        /// <summary>
        /// The default parameterless constructor.
        /// </summary>
        public GetLabelsRequest()
        { }

        /// <summary>
        /// Builds from content item type and content item ID.
        /// </summary>
        /// <param name="contentItemId">The ID of the content item to update the label for.</param>
        /// <param name="contentItemType">The label content type from <see cref="LabelContentTypes"/>.</param>        
        public GetLabelsRequest(Guid contentItemId, string contentItemType)
        {
            ContentList = new[] { new ContentType(contentItemId, contentItemType) };
        }

        /// <summary>
        /// The list of content items to get get labels for.
        /// </summary>        
        [XmlArray("contentList")]
        [XmlArrayItem("content")]
        public ContentType[] ContentList { get; set; } = Array.Empty<ContentType>();

        /// <summary>
        /// The class represent content in the request.
        /// </summary>
        [XmlRoot(ElementName = "content")]
        public class ContentType
        {
            /// <summary>
            /// The default parameterless constructor.
            /// </summary>
            public ContentType()
            { }

            /// <summary>
            /// Builds from content item type and content item ID.
            /// </summary>
            /// <param name="contentItemId">The ID of the content item to update the label for.</param>
            /// <param name="contentItemType">The label content type from <see cref="LabelContentTypes"/>.</param>   

            public ContentType(Guid contentItemId, string contentItemType)
            {
                ContentItemId = Guard.AgainstDefaultValue(contentItemId, () => contentItemId);
                ContentItemType = Guard.AgainstNullOrEmpty(contentItemType, () => contentItemType);
            }

            /// <summary>
            /// The ID of the content item to get labels for.
            /// </summary>
            [XmlAttribute("id")]
            public Guid ContentItemId { get; set; }

            /// <summary>
            /// The type of content item. It can be one of <see cref="LabelContentTypes"/>.
            /// </summary>
            [XmlAttribute("contentType")]
            public string? ContentItemType { get; set; }
        }
    }
}
