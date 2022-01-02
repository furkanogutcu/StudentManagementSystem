using System.Collections.Generic;
using StudentManagementSystem.Core.Utilities.Results;
using StudentManagementSystem.Entities.Concrete;

namespace StudentManagementSystem.Business.Abstract
{
    public interface IDepartmentService : IEntityCrudService<Department>
    {
        IDataResult<Department> GetByDepartmentNo(int departmentNo);
        IDataResult<List<Department>> GetAllByDepartmentNo(int departmentNo);
        IDataResult<List<Department>> GetAllContainDepartmentName(string departmentName);
        IDataResult<List<int>> GetAListUpToTheMaxNumberOfSemester();
    }
}
