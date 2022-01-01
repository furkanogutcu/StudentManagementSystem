using System.Collections.Generic;
using StudentManagementSystem.Business.Abstract;
using StudentManagementSystem.Core.Utilities.Results;
using StudentManagementSystem.DataAccess.Abstract;
using StudentManagementSystem.Entities.Concrete;

namespace StudentManagementSystem.Business.Concrete
{
    public class InstructorManager : IInstructorService
    {
        private readonly IInstructorDal _instructorDal;

        public InstructorManager(IInstructorDal instructorDal)
        {
            _instructorDal = instructorDal;
        }

        public IDataResult<List<Instructor>> GetAll()
        {
            return _instructorDal.GetAll(null);
        }

        public IDataResult<List<Instructor>> GetAllByDepartmentNo(int departmentNo)
        {
            return _instructorDal.GetAll(new Dictionary<string, dynamic>() { { "bolum_no", departmentNo } });
        }

        public IDataResult<Instructor> GetByInstructorNo(int instructorNo)
        {
            return _instructorDal.Get(new Dictionary<string, dynamic>() { { "ogretim_uye_no", instructorNo } });
        }

        public IResult Add(Instructor entity)
        {
            throw new System.NotImplementedException();
        }

        public IResult Update(Instructor entity)
        {
            throw new System.NotImplementedException();
        }

        public IResult Delete(Instructor entity)
        {
            throw new System.NotImplementedException();
        }
    }
}
