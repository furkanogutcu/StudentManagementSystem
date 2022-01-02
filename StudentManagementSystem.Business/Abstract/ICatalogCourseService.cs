using System.Collections.Generic;
using StudentManagementSystem.Core.Utilities.Results;
using StudentManagementSystem.Entities.Concrete;

namespace StudentManagementSystem.Business.Abstract
{
    public interface ICatalogCourseService : IEntityCrudService<CatalogCourse>
    {
        IDataResult<CatalogCourse> GetByCourseNo(int courseNo);
        IDataResult<List<CatalogCourse>> GetAllByCourseNo(int courseNo);
        IDataResult<List<CatalogCourse>> GetAllContainCourseName(string courseName);
    }
}
