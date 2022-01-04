using System.Collections.Generic;
using System.Linq;
using StudentManagementSystem.Business.Abstract;
using StudentManagementSystem.Business.ValidationRules.FluentValidation;
using StudentManagementSystem.Core.Utilities.Results;
using StudentManagementSystem.DataAccess.Abstract;
using StudentManagementSystem.Entities.Concrete;

namespace StudentManagementSystem.Business.Concrete
{
    public class DepartmentManager : CrudOperations<Department>, IDepartmentService
    {
        private readonly IDepartmentDal _departmentDal;
        private readonly DepartmentValidator _departmentValidator = new DepartmentValidator();

        public DepartmentManager(IDepartmentDal departmentDal) : base(typeof(DepartmentValidator), departmentDal)
        {
            _departmentDal = departmentDal;
        }

        public new IDataResult<List<Department>> GetAll()
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
    }
}
