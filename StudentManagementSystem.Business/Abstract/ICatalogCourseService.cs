using System.Collections.Generic;
using StudentManagementSystem.Core.Utilities.Results;
using StudentManagementSystem.Entities.Concrete;

namespace StudentManagementSystem.Business.Abstract
{
    public interface ICatalogCourseService : IEntityCrudService<CatalogCourse>
    {
        IDataResult<List<CatalogCourse>> GetAllByCourseNo(int courseNo);
        IDataResult<List<CatalogCourse>> GetAllByDepartmentNo(int departmentNo);
        IDataResult<List<CatalogCourse>> GetAllByInstructorNo(int instructorNo);
        IDataResult<List<CatalogCourse>> GetAllContainCourseName(string courseName);
        IDataResult<List<CatalogCourse>> GetAllByDepartmentNoAndSemesterNo(int departmentNo, int semesterNo);
        IDataResult<CatalogCourse> GetByCourseNo(int courseNo);
        IResult AddWithDepartmentTotalSemester(CatalogCourse catalogCourse, int departmentTotalSemester);
        IResult UpdateWithDepartmentTotalSemester(CatalogCourse catalogCourse, int departmentTotalSemester);
    }
}
