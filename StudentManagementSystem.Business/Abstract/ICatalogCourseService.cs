using System.Collections.Generic;
using StudentManagementSystem.Core.Utilities.Results;
using StudentManagementSystem.Entities.Concrete;

namespace StudentManagementSystem.Business.Abstract
{
    public interface ICatalogCourseService : IEntityCrudService<CatalogCourse>
    {
        IDataResult<List<CatalogCourse>> GetAllByCourseNo(int courseNo);
        IDataResult<List<CatalogCourse>> GetAllByDepartmentNo(int departmentNo);
        IDataResult<List<CatalogCourse>> GetAllContainCourseName(string courseName);
        IDataResult<CatalogCourse> GetByCourseNo(int courseNo);
    }
}
