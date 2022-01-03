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
    public class EnrolledCourseManager : IEnrolledCourseService
    {
        private readonly IEnrolledCourseDal _enrolledCourseDal;
        private readonly EnrolledCourseValidator _enrolledCourseValidator = new EnrolledCourseValidator();

        public EnrolledCourseManager(IEnrolledCourseDal enrolledCourseDal)
        {
            _enrolledCourseDal = enrolledCourseDal;
        }

        public IDataResult<List<EnrolledCourse>> GetAll()
        {
            return _enrolledCourseDal.GetAll(null);
        }

        public IDataResult<List<EnrolledCourse>> GetAllByStudentNo(int studentNo)
        {
            if (studentNo > 0)
            {
                return _enrolledCourseDal.GetAll(new Dictionary<string, dynamic>() { { "ogrenci_no", studentNo } });
            }

            return new ErrorDataResult<List<EnrolledCourse>>("Öğrenci no 0'dan büyük olmalıdır");
        }

        public IDataResult<EnrolledCourse> GetByCourseNoAndStudentNo(int courseNo, int studentNo)
        {
            if (courseNo > 0 && studentNo > 0)
            {
                return _enrolledCourseDal.Get(new Dictionary<string, dynamic>()
                    {{"ders_no", courseNo}, {"ogrenci_no", studentNo}});
            }

            return new ErrorDataResult<EnrolledCourse>("Ders no ve öğrenci no 0'dan büyük olmalıdır");
        }

        public IResult Add(EnrolledCourse entity)
        {
            throw new System.NotImplementedException();
        }

        public IResult Update(EnrolledCourse entity)
        {
            var validatorResult = ValidationTool.Validate(_enrolledCourseValidator, entity);
            if (validatorResult.Success)
            {
                return _enrolledCourseDal.Update(entity);
            }

            return new ErrorResult(ErrorMessageBuilder.CreateErrorMessageFromValidationFailure(validatorResult.Data));
        }

        public IResult Delete(EnrolledCourse entity)
        {
            throw new System.NotImplementedException();
        }
    }
}
