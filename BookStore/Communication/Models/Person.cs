using Communication.Models.Base;
using System.Runtime.Serialization;

namespace Communication.Models
{
    [DataContract]
    public class Person : BaseEntity
    {
        [DataMember]
        public string? FirstName { get; set; }

        [DataMember]
        public string? LastName { get; set; }

        [DataMember]
        public DateTime? DateOfBirth { get; set; }
    }
}
