using System.Collections.Generic;
using StudentManagementSystem.Core.Utilities.Results;
using StudentManagementSystem.Entities.Concrete;

namespace StudentManagementSystem.Business.Abstract
{
    public interface IStudentService : IEntityCrudService<Student>
    {
        IDataResult<List<Student>> GetAllByDepartmentNo(int departmentNo);
        IDataResult<List<Student>> GetAllByStudentNo(int studentNo);
        IDataResult<List<Student>> GetAllContainStudentName(string studentName);
        IDataResult<Student> GetByStudentNo(int studentNo);
    }
}
