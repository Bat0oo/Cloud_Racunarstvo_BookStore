using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Client.UI.Models
{
    [DataContract]
    public class PersonViewModel : Base.BaseEntityViewModel
    {
        [DataMember]
        public string? FirstName { get; set; }

        [DataMember]
        public string? LastName { get; set; }

        [DataMember]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DateOfBirth { get; set; }
    }
}
