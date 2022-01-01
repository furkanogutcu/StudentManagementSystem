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
    public class DepartmentManager : IDepartmentService
    {
        private readonly IDepartmentDal _departmentDal;
        private readonly DepartmentValidator _departmentValidator = new DepartmentValidator();

        public DepartmentManager(IDepartmentDal departmentDal)
        {
            _departmentDal = departmentDal;
        }

        public IDataResult<List<Department>> GetAll()
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

        public IResult Add(Department entity)
        {
            var validatorResult = ValidationTool.Validate(_departmentValidator, entity);
            if (validatorResult.Success)
            {
                return _departmentDal.Add(entity);
            }

            return new ErrorResult(ErrorMessageBuilder.CreateErrorMessageFromValidationFailure(validatorResult.Data));
        }

        public IResult Update(Department entity)
        {
            var validatorResult = ValidationTool.Validate(_departmentValidator, entity);
            if (validatorResult.Success)
            {
                return _departmentDal.Update(entity);
            }

            return new ErrorResult(ErrorMessageBuilder.CreateErrorMessageFromValidationFailure(validatorResult.Data));
        }

        public IResult Delete(Department entity)
        {
            var validatorResult = ValidationTool.Validate(_departmentValidator, entity);
            if (validatorResult.Success)
            {
                return _departmentDal.Delete(entity);
            }
            return new ErrorResult(ErrorMessageBuilder.CreateErrorMessageFromValidationFailure(validatorResult.Data));
        }
    }
}
