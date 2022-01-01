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
            var validatorResult = ValidationTool.Validate(_catalogCourseValidator, entity);
            if (validatorResult.Success)
            {
                return _catalogCourseDal.Update(entity);
            }

            return new ErrorResult(ErrorMessageBuilder.CreateErrorMessageFromValidationFailure(validatorResult.Data));
        }

        public IResult Delete(CatalogCourse entity)
        {
            throw new System.NotImplementedException();
        }
    }
}
