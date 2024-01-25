using System;
using Tableau.Migration.Content;

namespace Tableau.Migration.Api.Rest.Models
{
    /// <summary>
    /// The asset types for labels.
    /// </summary>
    public class LabelContentTypes : StringEnum<LabelContentTypes>
    {
        ///<summary>
        /// Gets the name of the database asset type for a label.
        ///</summary>
        public const string Database = "database";

        ///<summary>
        /// Gets the name of the table asset type for a label.
        ///</summary>
        public const string Table = "table";

        ///<summary>
        /// Gets the name of the column asset type for a label.
        ///</summary>
        public const string Column = "column";

        ///<summary>
        /// Gets the name of the datasource asset type for a label.
        ///</summary>
        public const string DataSource = "datasource";

        ///<summary>
        /// Gets the name of the flow asset type for a label.
        ///</summary>
        public const string Flow = "flow";

        ///<summary>
        /// Gets the name of the virtualconnection asset type for a label.
        ///</summary>
        public const string VirtualConnection = "virtualconnection";

        ///<summary>
        /// Gets the name of the virtualconnectiontable asset type for a label.
        ///</summary>
        public const string VirtualConnectionTable = "virtualconnectiontable";

        /// <summary>
        /// Gets the label content type value from the content type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string FromContentType(Type type)
        {
            if (string.Equals(type.Name, nameof(IDataSource), StringComparison.OrdinalIgnoreCase)
                || string.Equals(type.Name, nameof(IPublishableDataSource), StringComparison.OrdinalIgnoreCase))
            {
                return DataSource;
            }

            throw new NotSupportedException($"{type.Name} is not supported.");
        }
    }
}
