using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    [DataContract]
    public class Book : BaseEntity
    {
        [DataMember]
        public string? Title { get; set; }

        [DataMember]
        public string? Author { get; set; }

        [DataMember]
        public int? PagesNumber { get; set; }

        [DataMember]
        public int? PublicationYear { get; set; }

        [DataMember]
        public double? Price { get; set; }

        [DataMember]
        public int? Quantity { get; set; }
    }
}
