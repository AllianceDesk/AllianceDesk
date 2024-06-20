using System.Collections.Generic;

namespace ASI.Basecode.Data.Models
{
    public partial class Role
    {
        public int RoleId { get; set; }
        public string Name { get; set; }

        public List<User> Users { get; set; } = new List<User>();
    }
}