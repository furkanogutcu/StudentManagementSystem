using System.Collections.Generic;
using StudentManagementSystem.Business.Abstract;
using StudentManagementSystem.Business.ValidationRules.FluentValidation;
using StudentManagementSystem.Core.CrossCuttingConcerns.Validation.FluentValidation;
using StudentManagementSystem.Core.Utilities.Results;
using StudentManagementSystem.Core.Utilities.Validation;
using StudentManagementSystem.DataAccess.Abstract;
using StudentManagementSystem.Entities.Concrete;

namespace StudentManagementSystem.Business.Concrete
{
    public class CatalogCourseManager : ICatalogCourseService
    {
        private readonly ICatalogCourseDal _catalogCourseDal;
        private readonly CatalogCourseValidator _catalogCourseValidator = new CatalogCourseValidator();

        public CatalogCourseManager(ICatalogCourseDal catalogCourseDal)
        {
            _catalogCourseDal = catalogCourseDal;
        }

        public IDataResult<List<CatalogCourse>> GetAll()
        {
            return _catalogCourseDal.GetAll(null);
        }

        public IDataResult<List<CatalogCourse>> GetAllByDepartmentNoAndSemesterNo(int departmentNo, int semesterNo)
        {
            if (departmentNo > 0 && semesterNo > 0)
            {
                return _catalogCourseDal.GetAll(new Dictionary<string, dynamic>() { { "bolum_no", departmentNo }, { "ders_donemi", semesterNo } });
            }

            return new ErrorDataResult<List<CatalogCourse>>("Bölüm no ve ders dönemi 0'dan büyük olmalıdır");
        }

        public IDataResult<CatalogCourse> GetByCourseNo(int courseNo)
        {
            return _catalogCourseDal.Get(new Dictionary<string, dynamic>() { { "ders_no", courseNo } });
        }

        public IDataResult<List<CatalogCourse>> GetAllByCourseNo(int courseNo)
        {
            return _catalogCourseDal.GetAll(new Dictionary<string, dynamic>() { { "ders_no", courseNo } });
        }

        public IDataResult<List<CatalogCourse>> GetAllByDepartmentNo(int departmentNo)
        {
            return _catalogCourseDal.GetAll(new Dictionary<string, dynamic>() { { "bolum_no", departmentNo } });
        }

        public IDataResult<List<CatalogCourse>> GetAllContainCourseName(string courseName)
        {
            var courseResult = _catalogCourseDal.GetAll(null);
            if (courseResult.Success)
            {
                var returnList = new List<CatalogCourse>();
                foreach (var course in courseResult.Data)
                {
                    if (course.CourseName.ToUpper().Contains(courseName.ToUpper()))
                    {
                        returnList.Add(course);
                    }
                }

                return new SuccessDataResult<List<CatalogCourse>>(returnList);
            }
            return new ErrorDataResult<List<CatalogCourse>>("Dersler alınırken bir hata oluştu");
        }

        public IResult Add(CatalogCourse entity)
        {
            var validatorResult = ValidationTool.Validate(_catalogCourseValidator, entity);
            if (validatorResult.Success)
            {
                return _catalogCourseDal.Add(entity);
            }

            return new ErrorResult(ErrorMessageBuilder.CreateErrorMessageFromValidationFailure(validatorResult.Data));
        }

        public IResult Update(CatalogCourse entity)
        {
            var validatorResult = ValidationTool.Validate(_catalogCourseValidator, entity);
            if (validatorResult.Success)
            {
                return _catalogCourseDal.Update(entity);
            }

            return new ErrorResult(ErrorMessageBuilder.CreateErrorMessageFromValidationFailure(validatorResult.Data));
        }

        public IResult Delete(CatalogCourse entity)
        {
            var validatorResult = ValidationTool.Validate(_catalogCourseValidator, entity);
            if (validatorResult.Success)
            {
                return _catalogCourseDal.Delete(entity);
            }

            return new ErrorResult(ErrorMessageBuilder.CreateErrorMessageFromValidationFailure(validatorResult.Data));
        }
    }
}
