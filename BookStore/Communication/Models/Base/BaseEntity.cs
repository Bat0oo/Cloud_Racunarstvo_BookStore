using System.Runtime.Serialization;

namespace Communication.Models.Base
{
    [DataContract]
    public class BaseEntity
    {
        [DataMember]
        public long? Id { get; set; }
    }
}
