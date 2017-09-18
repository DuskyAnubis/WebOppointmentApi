using System.Collections.Generic;

namespace WebOppointmentApi.Models
{
    public class Orgnazition
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string OrgTypeCode { get; set; }
        public string OrgTypeName { get; set; }
        public int Parent { get; set; }
        public string Tel { get; set; }
        public string Address { get; set; }
        public string Info { get; set; }
        public string KeyWord { get; set; }
        public string LogoUrl { get; set; }
        public string PicUrl { get; set; }
        public string Status { get; set; }

        public ICollection<User> Users { get; set; }
    }
}
