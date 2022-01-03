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
    public class InstructorManager : IInstructorService
    {
        private readonly IInstructorDal _instructorDal;
        private readonly ICatalogCourseService _catalogCourseService;
        private readonly InstructorValidator _instructorValidator = new InstructorValidator();

        public InstructorManager(IInstructorDal instructorDal, ICatalogCourseService catalogCourseService)
        {
            _instructorDal = instructorDal;
            _catalogCourseService = catalogCourseService;
        }

        public IDataResult<List<Instructor>> GetAll()
        {
            return _instructorDal.GetAll(null);
        }

        public IDataResult<List<Instructor>> GetAllByDepartmentNo(int departmentNo)
        {
            if (departmentNo > 0)
            {
                return _instructorDal.GetAll(new Dictionary<string, dynamic>() { { "bolum_no", departmentNo } });
            }

            return new ErrorDataResult<List<Instructor>>("Bölüm no 0'dan büyük olmalıdır.");
        }

        public IDataResult<List<Instructor>> GetAllByInstructorNo(int instructorNo)
        {
            if (instructorNo > 0)
            {
                return _instructorDal.GetAll(new Dictionary<string, dynamic>() { { "ogretim_uye_no", instructorNo } });
            }

            return new ErrorDataResult<List<Instructor>>("Öğretim görevlisi no 0'dan büyük olmalıdır.");
        }

        public IDataResult<List<Instructor>> GetAllContainInstructorName(string instructorName)
        {
            var instructorResult = _instructorDal.GetAll(null);
            if (!instructorResult.Success)
            {
                return new ErrorDataResult<List<Instructor>>("Öğretim görevlisi listesi alınırken bir hata oluştu");
            }
            var returnList = new List<Instructor>();
            foreach (var instructor in instructorResult.Data)
            {
                if (instructor.FirstName.ToUpper().Contains(instructorName.ToUpper()) || instructor.LastName.ToUpper().Contains(instructorName.ToUpper()))
                {
                    returnList.Add(instructor);
                }
            }

            return new SuccessDataResult<List<Instructor>>(returnList);
        }

        public IDataResult<Instructor> GetByInstructorNo(int instructorNo)
        {
            return _instructorDal.Get(new Dictionary<string, dynamic>() { { "ogretim_uye_no", instructorNo } });
        }

        public IDataResult<Instructor> GetByCourseNo(int courseNo)
        {
            if (courseNo > 0)
            {
                var catalogCourseResult = _catalogCourseService.GetByCourseNo(courseNo);
                if (catalogCourseResult.Success)
                {
                    return GetByInstructorNo(catalogCourseResult.Data.InstructorNo);
                }

                return new ErrorDataResult<Instructor>("Ders detayları alınırken bir sorun oldu");
            }

            return new ErrorDataResult<Instructor>("Ders no 0'dan büyük olmalıdır");
        }

        public IResult Add(Instructor entity)
        {
            var validatorResult = ValidationTool.Validate(_instructorValidator, entity);
            if (validatorResult.Success)
            {
                return _instructorDal.Add(entity);
            }
            return new ErrorResult(ErrorMessageBuilder.CreateErrorMessageFromValidationFailure(validatorResult.Data));
        }

        public IResult Update(Instructor entity)
        {
            var validatorResult = ValidationTool.Validate(_instructorValidator, entity);
            if (validatorResult.Success)
            {
                return _instructorDal.Update(entity);
            }

            return new ErrorResult(ErrorMessageBuilder.CreateErrorMessageFromValidationFailure(validatorResult.Data));
        }

        public IResult Delete(Instructor entity)
        {
            var validatorResult = ValidationTool.Validate(_instructorValidator, entity);
            if (validatorResult.Success)
            {
                return _instructorDal.Delete(entity);
            }

            return new ErrorResult(ErrorMessageBuilder.CreateErrorMessageFromValidationFailure(validatorResult.Data));
        }
    }
}
