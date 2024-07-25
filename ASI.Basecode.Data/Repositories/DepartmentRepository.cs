using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Basecode.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Repositories
{
    public class DepartmentRepository : BaseRepository, IDepartmentRepository
    {

        public DepartmentRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

        public IEnumerable<Department> RetrieveAll()
        {
            return this.GetDbSet<Department>();
        }

        public Department GetDepartmentById(byte departmentId)
        {
            return this.GetDbSet<Department>().FirstOrDefault(x => x.DepartmentId == departmentId);
        }
    }
}
