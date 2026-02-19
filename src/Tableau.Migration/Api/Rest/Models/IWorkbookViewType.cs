using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tableau.Migration.Api.Rest.Models
{
    /// <summary>
    /// Interface for a view REST response with a workbook reference.
    /// </summary>
    public interface IWorkbookViewType : IRestIdentifiable, INamedContent, IWithTagTypes
    {
        /// <summary>
        /// The content URL for the response.
        /// </summary>
        public string? ContentUrl { get; }

        /// <summary>
        /// The created timestamp for the response.
        /// </summary>        
        public string? CreatedAt { get; }

        /// <summary>
        /// The updated timestamp for the response.
        /// </summary>
        public string? UpdatedAt { get; }

        /// <summary>
        /// The View URL Name for the response.
        /// </summary>
        public string? ViewUrlName { get; }
    }
}
