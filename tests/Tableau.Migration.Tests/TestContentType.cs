using System;

namespace Tableau.Migration.Tests
{
    public class TestContentType : IContentReference
    {
        public Guid Id { get; set; }

        public string ContentUrl { get; set; } = string.Empty;

        public ContentLocation Location { get; set; }

        public string Name => Location.Name;

        public bool Equals(IContentReference? other)
        {
            throw new NotImplementedException();
        }
    }

    public class OtherTestContentType : IContentReference
    {
        public Guid Id { get; set; }

        public string ContentUrl { get; set; } = string.Empty;

        public ContentLocation Location { get; set; }

        public string Name => Location.Name;

        public bool Equals(IContentReference? other)
        {
            throw new NotImplementedException();
        }
    }
}
