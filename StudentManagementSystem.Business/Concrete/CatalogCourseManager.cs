using System;
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
    public class CatalogCourseManager : CrudOperation<CatalogCourse>, ICatalogCourseService
    {
        private readonly ICatalogCourseDal _catalogCourseDal;
        private readonly CatalogCourseValidator _catalogCourseValidator = new CatalogCourseValidator();
        private readonly IEnrolledCourseService _enrolledCourseService = new EnrolledCourseManager(new SqlEnrolledCourseDal()); //FIXME
        private readonly IAdviserApprovalService _adviserApprovalService = new AdviserApprovalManager(new SqlAdviserApprovalDal());  //FIXME


        public CatalogCourseManager(ICatalogCourseDal catalogCourseDal) : base(typeof(CatalogCourseValidator), catalogCourseDal)
        {
            _catalogCourseDal = catalogCourseDal;
        }

        public override IDataResult<List<CatalogCourse>> GetAll()
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

        public IDataResult<List<CatalogCourse>> GetAllByInstructorNo(int instructorNo)
        {
            return _catalogCourseDal.GetAll(new Dictionary<string, dynamic>() { { "ogretim_uyesi_no", instructorNo } });
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

        public override IResult Add(CatalogCourse entity)
        {
            throw new NotImplementedException();
        }

        //The reason for not getting the total number of semester automatically is because we don't want to create a dependency infinite loop.
        public IResult AddWithDepartmentTotalSemester(CatalogCourse entity, int departmentTotalSemester)  
        {
            var validatorResult = ValidationTool.Validate(_catalogCourseValidator, entity);
            if (validatorResult.Success)
            {
                if (departmentTotalSemester < 1)
                {
                    return new ErrorResult("Toplam dönem sayısı 0'dan büyük olmalıdır");
                }

                var departmentSemester = departmentTotalSemester;

                if (entity.CourseSemester > departmentSemester)
                {
                    return new ErrorResult(
                        $"Dersin dönemi ({entity.CourseSemester}), bölümün dönem sayısından ({departmentSemester}) büyük olamaz");
                }

                entity.ModifiedAt = null;
                entity.DeletedAt = null;
                return _catalogCourseDal.Add(entity);
            }

            return new ErrorResult(ErrorMessageBuilder.CreateErrorMessageFromValidationFailure(validatorResult.Data));
        }

        public override IResult Update(CatalogCourse entity)
        {
            throw new NotImplementedException();
        }

        //The reason for not getting the total number of semester automatically is because we don't want to create a dependency infinite loop.
        public IResult UpdateWithDepartmentTotalSemester(CatalogCourse entity, int departmentTotalSemester)
        {
            var validatorResult = ValidationTool.Validate(_catalogCourseValidator, entity);
            if (validatorResult.Success)
            {
                if (departmentTotalSemester < 1)
                {
                    return new ErrorResult("Toplam dönem sayısı 0'dan büyük olmalıdır");
                }

                var departmentSemester = departmentTotalSemester;

                if (entity.CourseSemester > departmentSemester)
                {
                    return new ErrorResult(
                        $"Dersin dönemi ({entity.CourseSemester}), bölümün dönem sayısından ({departmentSemester}) büyük olamaz");
                }
                return _catalogCourseDal.Update(entity);
            }

            return new ErrorResult(ErrorMessageBuilder.CreateErrorMessageFromValidationFailure(validatorResult.Data));
        }

        public override IResult Delete(CatalogCourse entity)
        {
            var validatorResult = ValidationTool.Validate(_catalogCourseValidator, entity);
            if (validatorResult.Success)
            {
                var enrolledCoursesResult = _enrolledCourseService.GetAllByCourseNo(entity.CourseNo);
                if (!enrolledCoursesResult.Success)
                {
                    return new ErrorResult(enrolledCoursesResult.Message);
                }

                var courseNumber = enrolledCoursesResult.Data.Count;

                if (courseNumber != 0)
                {
                    return new ErrorResult(
                        $"Bu dersi alan {courseNumber} öğrenci bulunmaktadır. Öğrencilerin ders kayıtlarını silmeden bu dersi silemezsiniz");
                }

                var adviserApprovalResult = _adviserApprovalService.GetAllByCourseNo(entity.CourseNo);
                if (!adviserApprovalResult.Success)
                {
                    return new ErrorResult(adviserApprovalResult.Message);
                }

                var approvalNumber = adviserApprovalResult.Data.Count;

                if (approvalNumber != 0)
                {
                    return new ErrorResult(
                        $"Bu ders için danışman onayı bekleyen {approvalNumber} kayıt bulunmaktadır. Bu kayıtları silmeden bu dersi silemezsiniz");
                }

                return _catalogCourseDal.Delete(entity);
            }

            return new ErrorResult(ErrorMessageBuilder.CreateErrorMessageFromValidationFailure(validatorResult.Data));
        }
    }
}
