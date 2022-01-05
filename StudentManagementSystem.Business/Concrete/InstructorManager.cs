using System.Collections.Generic;
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
    public class InstructorManager : CrudOperation<Instructor>, IInstructorService
    {
        private readonly IInstructorDal _instructorDal;
        private readonly InstructorValidator _instructorValidator = new InstructorValidator();
        private readonly ICatalogCourseService _catalogCourseService = new CatalogCourseManager(new SqlCatalogCourseDal());
        private readonly IStudentService _studentService = new StudentManager(new SqlStudentDal());

        public InstructorManager(IInstructorDal instructorDal) : base(typeof(InstructorValidator), instructorDal)
        {
            _instructorDal = instructorDal;
        }

        public override IDataResult<List<Instructor>> GetAll()
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

        public override IResult Delete(Instructor entity)
        {
            var validatorResult = ValidationTool.Validate(_instructorValidator, entity);
            if (validatorResult.Success)
            {
                var coursesResult = _catalogCourseService.GetAllByInstructorNo(entity.InstructorNo);
                if (!coursesResult.Success)
                {
                    return new ErrorResult(coursesResult.Message);
                }

                var numberOfCourses = coursesResult.Data.Count;
                if (numberOfCourses != 0)
                {
                    return new ErrorResult(
                        $"Öğretim görevlisi {numberOfCourses} dersin öğretim görevlisidir. Bu derslerin öğretim görevlilerini değiştirmeden öğretim görevlisini silemezsiniz");
                }

                var studentsResult = _studentService.GetAllByAdvisorNo(entity.InstructorNo);
                if (!studentsResult.Success)
                {
                    return new ErrorResult(studentsResult.Message);
                }

                var adviserNumber = studentsResult.Data.Count;

                if (adviserNumber != 0)
                {
                    return new ErrorResult(
                        $"Öğretim görevlisi {adviserNumber} öğrencinin danışmanıdır. Bu öğrencilerin danışmanlarını değiştirmeden öğretim görevlisini silemezsiniz");
                }

                return _instructorDal.Delete(entity);
            }

            return new ErrorResult(ErrorMessageBuilder.CreateErrorMessageFromValidationFailure(validatorResult.Data));
        }
    }
}
