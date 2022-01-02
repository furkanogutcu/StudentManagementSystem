using System.Collections.Generic;
using StudentManagementSystem.Core.Utilities.Results;
using StudentManagementSystem.Entities.Concrete;

namespace StudentManagementSystem.Business.Abstract
{
    public interface IEnrolledCourseService : IEntityCrudService<EnrolledCourse>
    {
    }
}
