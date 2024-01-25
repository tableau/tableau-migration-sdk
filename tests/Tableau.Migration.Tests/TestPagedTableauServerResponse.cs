using System;
using Tableau.Migration.Api.Rest.Models.Responses;

namespace Tableau.Migration.Tests
{
    public class TestPagedTableauServerResponse : PagedTableauServerResponse<object>
    {
        public override object[] Items { get; set; } = Array.Empty<object>();
    }
}
