using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
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
