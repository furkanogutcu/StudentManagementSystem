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
    public class DepartmentManager : CrudOperation<Department>, IDepartmentService
    {
        private readonly IDepartmentDal _departmentDal;
        private readonly ICatalogCourseService _catalogCourseService = new CatalogCourseManager(new SqlCatalogCourseDal()); //FIXME
        private readonly IStudentService _studentService = new StudentManager(new SqlStudentDal()); //FIXME
        private readonly IInstructorService _instructorService = new InstructorManager(new SqlInstructorDal()); //FIXME
        private readonly DepartmentValidator _departmentValidator = new DepartmentValidator();

        public DepartmentManager(IDepartmentDal departmentDal) : base(typeof(DepartmentValidator), departmentDal)
        {
            _departmentDal = departmentDal;
        }

        public override IDataResult<List<Department>> GetAll()
        {
            return _departmentDal.GetAll(null);
        }

        public IDataResult<Department> GetByDepartmentNo(int departmentNo)
        {
            if (departmentNo > 0)
            {
                return _departmentDal.Get(new Dictionary<string, dynamic>() { { "bolum_no", departmentNo } });
            }

            return new ErrorDataResult<Department>("Bölüm no 0'dan büyük olmalıdır");
        }

        public IDataResult<List<Department>> GetAllByDepartmentNo(int departmentNo)
        {
            if (departmentNo > 0)
            {
                return _departmentDal.GetAll(new Dictionary<string, dynamic>() { { "bolum_no", departmentNo } });
            }

            return new ErrorDataResult<List<Department>>("Bölüm no 0'dan büyük olmalıdır");
        }

        public IDataResult<List<Department>> GetAllContainDepartmentName(string departmentName)
        {
            var departmentResult = _departmentDal.GetAll(null);
            if (!departmentResult.Success)
            {
                return new ErrorDataResult<List<Department>>("Bölümler alınırken bir hata oluştu");
            }
            var returnList = new List<Department>();
            foreach (var department in departmentResult.Data)
            {
                if (department.DepartmentName.ToUpper().Contains(departmentName.ToUpper()))
                {
                    returnList.Add(department);
                }
            }

            return new SuccessDataResult<List<Department>>(returnList);
        }

        public IDataResult<List<int>> GetAListUpToTheMaxNumberOfSemester()
        {
            var departmentResult = _departmentDal.GetAll(null);
            if (!departmentResult.Success)
            {
                return new ErrorDataResult<List<int>>("Bölümler alınırken bir hata oluştu");
            }

            if (departmentResult.Data.Count == 0)
            {
                return new SuccessDataResult<List<int>>(new List<int>());
            }

            var resultList = new List<int>();
            var maxSemesterNumber = departmentResult.Data.Max(c => c.NumberOfSemester);

            for (int i = 0; i < maxSemesterNumber; i++)
            {
                resultList.Add(i + 1);
            }

            return new SuccessDataResult<List<int>>(resultList);
        }

        public override IResult Delete(Department entity)
        {
            var validatorResult = ValidationTool.Validate(_departmentValidator, entity);
            if (validatorResult.Success)
            {
                // Check Courses
                var catalogCourseResult = _catalogCourseService.GetAllByDepartmentNo(entity.DepartmentNo);
                if (!catalogCourseResult.Success)
                {
                    return new ErrorResult(catalogCourseResult.Message);
                }

                var courseCount = catalogCourseResult.Data.Count;

                if (courseCount != 0)
                {
                    return new ErrorResult(
                        $"Bu bölüme kayıtlı {courseCount} ders bulunmaktadır. Bu dersleri silmeden bölümü silemezsiniz");
                }

                // Check Students
                var studentResult = _studentService.GetAllByDepartmentNo(entity.DepartmentNo);
                if (!studentResult.Success)
                {
                    return new ErrorResult(studentResult.Message);
                }

                var studentCount = studentResult.Data.Count();

                if (studentCount != 0)
                {
                    return new ErrorResult(
                        $"Bu bölüme kayıtlı {studentCount} öğrenci bulunmaktadır. Bu öğrencilerin kaydını silmeden bu bölümü silemezsiniz");
                }

                // Check Instructors

                var instructorResult = _instructorService.GetAllByDepartmentNo(entity.DepartmentNo);
                if (!instructorResult.Success)
                {
                    return new ErrorResult(instructorResult.Message);
                }

                var instructorCount = instructorResult.Data.Count;

                if (instructorCount != 0)
                {
                    return new ErrorResult(
                        $"Bu bölüme kayıtlı {instructorCount} öğretim görevlisi bulunmaktadır. Bu öğretim görevlilerinin kaydını silmeden bu bölümü silemezsiniz");
                }

                return _departmentDal.Delete(entity);
            }
            return new ErrorResult(ErrorMessageBuilder.CreateErrorMessageFromValidationFailure(validatorResult.Data));
        }
    }
}
