using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Models
{
    public partial class Department
    {
        public Department()
        {
            Teams = new HashSet<Team>();
        }

        public byte DepartmentId { get; set; }
        public string DepartmentName { get; set; }

        public virtual ICollection<Team> Teams { get; set; }
    }
}
