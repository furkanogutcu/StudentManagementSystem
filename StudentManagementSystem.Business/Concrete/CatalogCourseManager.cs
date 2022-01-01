using System.Collections.Generic;
using StudentManagementSystem.Business.Abstract;
using StudentManagementSystem.Core.Utilities.Results;
using StudentManagementSystem.DataAccess.Abstract;
using StudentManagementSystem.Entities.Concrete;

namespace StudentManagementSystem.Business.Concrete
{
    public class CatalogCourseManager : ICatalogCourseService
    {
        private readonly ICatalogCourseDal _catalogCourseDal;

        public CatalogCourseManager(ICatalogCourseDal catalogCourseDal)
        {
            _catalogCourseDal = catalogCourseDal;
        }

        public IDataResult<List<CatalogCourse>> GetAll()
        {
            return _catalogCourseDal.GetAll(null);
        }

        public IDataResult<CatalogCourse> GetByCourseNo(int courseNo)
        {
            return _catalogCourseDal.Get(new Dictionary<string, dynamic>() { { "ders_no", courseNo } });
        }

        public IResult Add(CatalogCourse entity)
        {
            throw new System.NotImplementedException();
        }

        public IResult Update(CatalogCourse entity)
        {
            throw new System.NotImplementedException();
        }

        public IResult Delete(CatalogCourse entity)
        {
            throw new System.NotImplementedException();
        }
    }
}
