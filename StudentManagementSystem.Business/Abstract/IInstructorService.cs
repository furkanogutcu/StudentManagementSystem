using System.Collections.Generic;
using StudentManagementSystem.Core.Utilities.Results;
using StudentManagementSystem.Entities.Concrete;

namespace StudentManagementSystem.Business.Abstract
{
    public interface IInstructorService : IEntityCrudService<Instructor>
    {
        IDataResult<List<Instructor>> GetAllByDepartmentNo(int departmentNo);
        IDataResult<Instructor> GetByInstructorNo(int instructorNo);
    }
}
