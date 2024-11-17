using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    [DataContract]
    public class BaseEntity
    {
        [DataMember]
        public long? Id { get; set; }
    }
}
