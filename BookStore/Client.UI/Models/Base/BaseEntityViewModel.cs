using System.Runtime.Serialization;

namespace Client.UI.Models.Base
{
    [DataContract]
    public class BaseEntityViewModel
    {
        [DataMember]
        public long? Id { get; set; }
    }
}
