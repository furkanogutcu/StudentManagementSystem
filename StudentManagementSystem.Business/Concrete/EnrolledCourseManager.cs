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
    public class EnrolledCourseManager : CrudOperation<EnrolledCourse>, IEnrolledCourseService
    {
        private readonly IEnrolledCourseDal _enrolledCourseDal;
        private readonly EnrolledCourseValidator _enrolledCourseValidator = new EnrolledCourseValidator();

        public EnrolledCourseManager(IEnrolledCourseDal enrolledCourseDal) : base(typeof(EnrolledCourseValidator), enrolledCourseDal)
        {
            _enrolledCourseDal = enrolledCourseDal;
        }

        public override IDataResult<List<EnrolledCourse>> GetAll()
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

        public IDataResult<List<EnrolledCourse>> GetAllByCourseNo(int courseNo)
        {
            if (courseNo > 0)
            {
                return _enrolledCourseDal.GetAll(new Dictionary<string, dynamic>() { { "ders_no", courseNo } });
            }

            return new ErrorDataResult<List<EnrolledCourse>>("Ders no 0'dan büyük olmalıdır");
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
        public override IResult Add(EnrolledCourse entity)
        {
            var validateResult = ValidationTool.Validate(_enrolledCourseValidator, entity);
            if (validateResult.Success)
            {
                entity.ModifiedAt = null;
                entity.DeletedAt = null;
                entity.VizeResult = null;
                entity.FinalResult = null;
                entity.ButunlemeResult = null;
                return _enrolledCourseDal.Add(entity);
            }

            return new ErrorResult(ErrorMessageBuilder.CreateErrorMessageFromValidationFailure(validateResult.Data));
        }
    }
}
