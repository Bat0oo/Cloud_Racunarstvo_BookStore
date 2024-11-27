using System.Runtime.Serialization;

namespace Client.UI.Models
{
    [DataContract]
    public class ClientViewModel : PersonViewModel
    {
        [DataMember]
        public string? BankName { get; set; }

        [DataMember]
        public double? BankAccount { get; set; }

        [DataMember]
        public string? BankMembership { get; set; }
    }
}
