
using System.Collections.Generic;
using FluentValidation.Results;
using StudentManagementSystem.Business.Abstract;
using StudentManagementSystem.Business.ValidationRules.FluentValidation;
using StudentManagementSystem.Core.CrossCuttingConcerns.Validation.FluentValidation;
using StudentManagementSystem.Core.Utilities.Results;
using StudentManagementSystem.Core.Utilities.Validation;
using StudentManagementSystem.DataAccess.Abstract;
using StudentManagementSystem.Entities.Concrete;

namespace StudentManagementSystem.Business.Concrete
{
    public class OfficerManager : IOfficerService
    {
        private readonly IOfficerDal _officerDal;

        public OfficerManager(IOfficerDal officerDal)
        {
            _officerDal = officerDal;
        }

        public IDataResult<List<Officer>> GetAll()
        {
            throw new System.NotImplementedException();
        }

        public IDataResult<Officer> GetByOfficerNo(int officerNo)
        {
            return _officerDal.Get(new Dictionary<string, dynamic>() { { "memur_no", officerNo } });
        }

        public IResult Add(Officer entity)
        {
            throw new System.NotImplementedException();
        }

        public IResult Update(Officer entity)
        {
            var validatorResult = ValidationTool.Validate(new OfficerValidator(), entity);
            if (validatorResult.Success)
            {
                return _officerDal.Update(entity);
            }
            return new ErrorResult(ErrorMessageBuilder.CreateErrorMessageFromValidationFailure(validatorResult.Data));
            
        }

        public IResult Delete(Officer entity)
        {
            throw new System.NotImplementedException();
        }
    }
}
