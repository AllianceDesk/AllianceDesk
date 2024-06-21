using System.Collections.Generic;

namespace ASI.Basecode.Data.Models
{
    public partial class UserRole
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public List<User> Users { get; set; } = new List<User>();
    }
}