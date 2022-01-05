using System.Collections.Generic;
using StudentManagementSystem.Core.Utilities.Results;
using StudentManagementSystem.Entities.Concrete;

namespace StudentManagementSystem.Business.Abstract
{
    public interface IEnrolledCourseService : IEntityCrudService<EnrolledCourse>
    {
        IDataResult<List<EnrolledCourse>> GetAllByStudentNo(int studentNo);
        IDataResult<List<EnrolledCourse>> GetAllByCourseNo(int courseNo);
        IDataResult<EnrolledCourse> GetByCourseNoAndStudentNo(int courseNo, int studentNo);
    }
}
