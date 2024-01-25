namespace Tableau.Migration.Api.Rest.Models
{
    /// <summary>
    /// The built-in categories for labels.
    /// </summary>
    public class LabelCategories : StringEnum<LabelCategories>
    {
        ///<summary>
        /// Gets the name of the Certification category for a label.
        ///</summary>
        public const string Certification = "Certification";

        ///<summary>
        /// Gets the name of the Data Quality Warning category for a label.
        ///</summary>
        public const string DataQualityWarning = "DataQualityWarning";

        ///<summary>
        /// Gets the name of the Sensitivity category for a label.
        ///</summary>
        public const string Sensitivity = "Sensitivity";
    }
}
