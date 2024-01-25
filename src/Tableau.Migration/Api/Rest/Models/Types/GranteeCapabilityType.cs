using System;
using System.Linq;
using System.Xml.Serialization;
using Tableau.Migration.Content.Permissions;

namespace Tableau.Migration.Api.Rest.Models.Types
{
    /// <summary>
    /// Class that defines the Grantee Capability element for the Tableau REST API XML response.
    /// </summary>
    public class GranteeCapabilityType
    {
        internal GranteeCapabilityType()
        { }

        /// <summary>
        /// Constructor to build from <see cref="IGranteeCapability"/>.
        /// </summary>
        /// <param name="granteeCapability"></param>
        public GranteeCapabilityType(IGranteeCapability granteeCapability)
        {
            switch (granteeCapability.GranteeType)
            {
                case GranteeType.Group:
                    {
                        Group = new GroupType()
                        {
                            Id = granteeCapability.GranteeId
                        };
                        break;
                    }
                case GranteeType.User:
                    {
                        User = new UserType()
                        {
                            Id = granteeCapability.GranteeId
                        };
                        break;
                    }
            };

            Capabilities = granteeCapability
                .Capabilities
                .Select(c => new CapabilityType(c))
                .ToArray();
        }

        /// <summary>
        /// The group element if present.
        /// </summary>
        [XmlElement("group")]
        public GroupType? Group { get; set; }

        /// <summary>
        /// The user element if present.
        /// </summary>
        [XmlElement("user")]
        public UserType? User { get; set; }

        /// <summary>
        /// The collection of grantee capabilities.
        /// </summary>
        [XmlArray("capabilities")]
        [XmlArrayItem("capability")]
        public CapabilityType[] Capabilities { get; set; } = Array.Empty<CapabilityType>();

        /// <summary>
        /// Gets the ID of the grantee.
        /// </summary>
        [XmlIgnore]
        public Guid GranteeId
            => Group?.Id ?? User?.Id ?? throw new InvalidOperationException("Could not determine grantee ID");

        /// <summary>
        /// Gets the type of grantee.
        /// </summary>
        [XmlIgnore]
        public GranteeType GranteeType
            => Group is not null ? GranteeType.Group : User is not null ? GranteeType.User : throw new InvalidOperationException("Could not determine grantee type");

        /// <summary>
        /// Class that defines the group xml element.
        /// </summary>
        public class GroupType
        {
            /// <summary>
            /// The Group Id.
            /// </summary>
            [XmlAttribute("id")]
            public Guid Id { get; set; }
        }


        /// <summary>
        /// Class that defines the User xml element.
        /// </summary>
        public class UserType
        {
            /// <summary>
            /// The User Id.
            /// </summary>
            [XmlAttribute("id")]
            public Guid Id { get; set; }
        }
    }
}
