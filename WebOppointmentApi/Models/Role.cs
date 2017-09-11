using System.Collections.Generic;

namespace WebOppointmentApi.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }

        public ICollection<User> User { get; set; }
        public ICollection<RolePermission> RolePermission { get; set; }
    }
}
