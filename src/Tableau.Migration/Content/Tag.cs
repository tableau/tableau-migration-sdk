using Tableau.Migration.Api.Rest.Models;

namespace Tableau.Migration.Content
{
    /// <summary>
    /// 
    /// </summary>
    public class Tag : ITag
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="label"></param>
        public Tag(string label)
        {
            Label = Guard.AgainstNullOrEmpty(label, () => label);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        public Tag(ITagType response)
        {
            Label = Guard.AgainstNullOrEmpty(response.Label, () => response.Label);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public Tag(ITag item)
        {
            Label = Guard.AgainstNullOrEmpty(item.Label, () => item.Label);
        }

        /// <summary>
        /// 
        /// </summary>
        public string Label { get; set; }
    }
}