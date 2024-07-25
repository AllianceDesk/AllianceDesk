using System;
using System.Collections.Generic;
using ASI.Basecode.Data.Models;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IDepartmentRepository
    {
        IEnumerable<Department> RetrieveAll();

        Department GetDepartmentById(byte departmentId);
    }
}