using System.Collections.Generic;
using StudentManagementSystem.Business.Abstract;
using StudentManagementSystem.Core.Utilities.Results;
using StudentManagementSystem.DataAccess.Abstract;
using StudentManagementSystem.Entities.Concrete;

namespace StudentManagementSystem.Business.Concrete
{
    public class EnrolledCourseManager : IEnrolledCourseService
    {
        private readonly IEnrolledCourseDal _enrolledCourseDal;

        public EnrolledCourseManager(IEnrolledCourseDal enrolledCourseDal)
        {
            _enrolledCourseDal = enrolledCourseDal;
        }

        public IDataResult<List<EnrolledCourse>> GetAll()
        {
            return _enrolledCourseDal.GetAll(null);
        }

        public IResult Add(EnrolledCourse entity)
        {
            throw new System.NotImplementedException();
        }

        public IResult Update(EnrolledCourse entity)
        {
            throw new System.NotImplementedException();
        }

        public IResult Delete(EnrolledCourse entity)
        {
            throw new System.NotImplementedException();
        }
    }
}
