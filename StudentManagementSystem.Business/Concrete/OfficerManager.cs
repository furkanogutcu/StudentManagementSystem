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
    public class OfficerManager : CrudOperation<Officer>, IOfficerService
    {
        private readonly IOfficerDal _officerDal;
        private readonly OfficerValidator _officerValidator = new OfficerValidator();

        public OfficerManager(IOfficerDal officerDal) : base(typeof(OfficerValidator), officerDal)
        {
            _officerDal = officerDal;
        }

        public override IDataResult<List<Officer>> GetAll()
        {
            return _officerDal.GetAll(null);
        }

        public IDataResult<Officer> GetByOfficerNo(int officerNo)
        {
            return _officerDal.Get(new Dictionary<string, dynamic>() { { "memur_no", officerNo } });
        }
    }
}
