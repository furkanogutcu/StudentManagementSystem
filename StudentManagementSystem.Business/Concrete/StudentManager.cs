using System;
using System.Collections.Generic;
using System.Linq;
using StudentManagementSystem.Business.Abstract;
using StudentManagementSystem.Business.ValidationRules.FluentValidation;
using StudentManagementSystem.Core.CrossCuttingConcerns.Validation.FluentValidation;
using StudentManagementSystem.Core.Utilities.Results;
using StudentManagementSystem.Core.Utilities.Validation;
using StudentManagementSystem.DataAccess.Abstract;
using StudentManagementSystem.DataAccess.Concrete.Sql;
using StudentManagementSystem.Entities.Concrete;

namespace StudentManagementSystem.Business.Concrete
{
    public class StudentManager : CrudOperation<Student>, IStudentService
    {
        private readonly IStudentDal _studentDal;
        private readonly StudentValidator _studentValidator = new StudentValidator();
        private readonly IEnrolledCourseService _enrolledCourseService = new EnrolledCourseManager(new SqlEnrolledCourseDal()); //FIXME
        private readonly IAdviserApprovalService _adviserApprovalService = new AdviserApprovalManager(new SqlAdviserApprovalDal()); //FIXME

        public StudentManager(IStudentDal studentDal) : base(typeof(StudentValidator), studentDal)
        {
            _studentDal = studentDal;
        }

        public override IDataResult<List<Student>> GetAll()
        {
            return _studentDal.GetAll(null);
        }

        public IDataResult<List<Student>> GetAllByDepartmentNo(int departmentNo)
        {
            return _studentDal.GetAll(new Dictionary<string, dynamic>() { { "bolum_no", departmentNo } });
        }

        public IDataResult<List<Student>> GetAllByStudentNo(int studentNo)
        {
            if (studentNo > 0)
            {
                return _studentDal.GetAll(new Dictionary<string, dynamic>() { { "ogrenci_no", studentNo } });
            }

            return new ErrorDataResult<List<Student>>("Öğrenci no 0'dan büyük olmalıdır");
        }

        public IDataResult<List<Student>> GetAllByAdvisorNo(int adviserNo)
        {
            if (adviserNo > 0)
            {
                return _studentDal.GetAll(new Dictionary<string, dynamic>() { { "danisman_no", adviserNo } });
            }

            return new ErrorDataResult<List<Student>>("Danışman no 0'dan büyük olmalıdır");
        }

        public IDataResult<List<Student>> GetAllContainStudentName(string studentName)
        {
            var studentResult = _studentDal.GetAll(null);
            if (!studentResult.Success)
            {
                return new ErrorDataResult<List<Student>>("Öğrenciler alınırken bir hata oluştu");
            }
            var returnList = new List<Student>();
            foreach (var student in studentResult.Data)
            {
                if (student.FirstName.ToUpper().Contains(studentName.ToUpper()) || student.LastName.ToUpper().Contains(studentName.ToUpper()))
                {
                    returnList.Add(student);
                }
            }

            return new SuccessDataResult<List<Student>>(returnList);
        }

        public IDataResult<List<Student>> GetAllByCourseNo(int courseNo)
        {
            var enrolledCourseResult = _enrolledCourseService.GetAll();
            if (enrolledCourseResult.Success)
            {
                var courseStudents = enrolledCourseResult.Data.Where(e => e.CourseNo == courseNo).ToList();
                if (courseStudents.Count > 0)
                {
                    var returnList = new List<Student>();
                    foreach (var enrolledCourse in courseStudents)
                    {
                        var studentResult = GetByStudentNo(enrolledCourse.StudentNo);
                        if (studentResult.Success)
                        {
                            returnList.Add(studentResult.Data);
                        }
                        else
                        {
                            return new ErrorDataResult<List<Student>>("Öğrenci bilgileri alınırken bir hata oldu");
                        }
                    }

                    return new SuccessDataResult<List<Student>>(returnList);
                }
                else
                {
                    return new SuccessDataResult<List<Student>>(new List<Student>());
                }
            }
            else
            {
                return new ErrorDataResult<List<Student>>("Ders kayıtları alınırken bir hata oluştu");
            }
        }

        public IDataResult<Student> GetByStudentNo(int studentNo)
        {
            return _studentDal.Get(new Dictionary<string, dynamic>() { { "ogrenci_no", studentNo } });
        }

        public override IResult Update(Student entity)
        {
            throw new NotImplementedException();
        }

        public IResult UpdateWithDepartmentTotalSemester(Student entity, int departmentTotalSemester)
        {
            var validatorResult = ValidationTool.Validate(_studentValidator, entity);
            if (validatorResult.Success)
            {
                var departmentSemester = departmentTotalSemester;

                if (entity.Semester > departmentSemester)
                {
                    return new ErrorResult(
                        $"Öğrencinin dönemi ({entity.Semester}), bölümünün toplam dönem sayısından ({departmentSemester}) büyük olamaz");
                }

                return _studentDal.Update(entity);
            }

            return new ErrorResult(ErrorMessageBuilder.CreateErrorMessageFromValidationFailure(validatorResult.Data));
        }

        public override IResult Delete(Student entity)
        {
            var validatorResult = ValidationTool.Validate(_studentValidator, entity);
            if (validatorResult.Success)
            {
                var enrolledResult = _enrolledCourseService.GetAllByStudentNo(entity.StudentNo);
                if (!enrolledResult.Success)
                {
                    return new ErrorResult(enrolledResult.Message);
                }

                var enrolledCourseNumber = enrolledResult.Data.Count;

                if (enrolledCourseNumber != 0)
                {
                    return new ErrorResult(
                        $"Öğrenci {enrolledCourseNumber} derse kayıtlı. Öğrencinin ders kayıtları silinmeden öğrenciyi silemezsiniz");
                }

                var approvalResult = _adviserApprovalService.GetAllByStudentNo(entity.StudentNo);
                if (!approvalResult.Success)
                {
                    return new ErrorResult(approvalResult.Message);
                }

                var approvalNumber = approvalResult.Data.Count;

                if (approvalNumber != 0)
                {
                    return new ErrorResult(
                        $"Öğrencinin {approvalNumber} ders için onay bekliyor. Öğrencinin onay bekleyen derslerini silmeden öğrenciyi silemezsiniz");
                }

                return _studentDal.Delete(entity);
            }

            return new ErrorResult(ErrorMessageBuilder.CreateErrorMessageFromValidationFailure(validatorResult.Data));
        }
    }
}
