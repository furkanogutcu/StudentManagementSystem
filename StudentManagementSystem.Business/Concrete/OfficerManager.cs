using System.Collections.Generic;
using StudentManagementSystem.Business.Abstract;
using StudentManagementSystem.Business.ValidationRules.FluentValidation;
using StudentManagementSystem.Core.Utilities.Results;
using StudentManagementSystem.DataAccess.Abstract;
using StudentManagementSystem.Entities.Concrete;

namespace StudentManagementSystem.Business.Concrete
{
    public class OfficerManager : CrudOperations<Officer>, IOfficerService
    {
        private readonly IOfficerDal _officerDal;

        public OfficerManager(IOfficerDal officerDal) : base(typeof(OfficerValidator), officerDal)
        {
            _officerDal = officerDal;
        }

        public new IDataResult<List<Officer>> GetAll()
        {
            return _officerDal.GetAll(null);
        }

        public IDataResult<Officer> GetByOfficerNo(int officerNo)
        {
            return _officerDal.Get(new Dictionary<string, dynamic>() { { "memur_no", officerNo } });
        }
    }
}
