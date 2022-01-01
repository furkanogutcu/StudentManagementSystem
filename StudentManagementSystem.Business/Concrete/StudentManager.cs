using System.Collections.Generic;
using StudentManagementSystem.Business.Abstract;
using StudentManagementSystem.Core.Utilities.Results;
using StudentManagementSystem.DataAccess.Abstract;
using StudentManagementSystem.Entities.Concrete;

namespace StudentManagementSystem.Business.Concrete
{
    public class StudentManager : IStudentService
    {
        private readonly IStudentDal _studentDal;

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
            return _studentDal.GetAll(new Dictionary<string, dynamic>() {{"bolum_no", departmentNo}});
        }

        public IResult Add(Student entity)
        {
            throw new System.NotImplementedException();
        }

        public IResult Update(Student entity)
        {
            throw new System.NotImplementedException();
        }

        public IResult Delete(Student entity)
        {
            throw new System.NotImplementedException();
        }
    }
}
