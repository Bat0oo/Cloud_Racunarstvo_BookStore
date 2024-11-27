using System.Runtime.Serialization;

namespace Communication.Models
{
    [DataContract]
    public class Client : Person
    {
        [DataMember]
        public string? BankName { get; set; }

        [DataMember]
        public double? BankAccount { get; set; }

        [DataMember]
        public string? BankMembership { get; set; }
    }
}
