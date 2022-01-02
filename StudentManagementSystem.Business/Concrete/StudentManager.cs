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
    public class StudentManager : IStudentService
    {
        private readonly IStudentDal _studentDal;
        private readonly StudentValidator _studentValidator = new StudentValidator();

        public StudentManager(IStudentDal studentDal)
        {
            _studentDal = studentDal;
        }

        public IDataResult<List<Student>> GetAll()
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

        public IDataResult<Student> GetByStudentNo(int studentNo)
        {
            return _studentDal.Get(new Dictionary<string, dynamic>() { { "ogrenci_no", studentNo } });
        }

        public IResult Add(Student entity)
        {
            var validatorResult = ValidationTool.Validate(_studentValidator, entity);
            if (validatorResult.Success)
            {
                return _studentDal.Add(entity);
            }

            return new ErrorResult(ErrorMessageBuilder.CreateErrorMessageFromValidationFailure(validatorResult.Data));
        }

        public IResult Update(Student entity)
        {
            var validatorResult = ValidationTool.Validate(_studentValidator, entity);
            if (validatorResult.Success)
            {
                return _studentDal.Update(entity);
            }

            return new ErrorResult(ErrorMessageBuilder.CreateErrorMessageFromValidationFailure(validatorResult.Data));
        }

        public IResult Delete(Student entity)
        {
            var validatorResult = ValidationTool.Validate(_studentValidator, entity);
            if (validatorResult.Success)
            {
                return _studentDal.Delete(entity);
            }

            return new ErrorResult(ErrorMessageBuilder.CreateErrorMessageFromValidationFailure(validatorResult.Data));
        }
    }
}
