using System;
using System.Collections.Generic;
using FluentValidation;
using StudentManagementSystem.Business.Abstract;
using StudentManagementSystem.Core.CrossCuttingConcerns.Validation.FluentValidation;
using StudentManagementSystem.Core.DataAccess.Sql;
using StudentManagementSystem.Core.Entities;
using StudentManagementSystem.Core.Utilities.Results;
using StudentManagementSystem.Core.Utilities.Validation;

namespace StudentManagementSystem.Business.Concrete
{
    public class CrudOperations<T> : IEntityCrudService<T>
        where T : class, IEntity, new()
    {
        private readonly IValidator _validator;
        private readonly IEntityRepository<T> _dal;
        public CrudOperations(Type validatorType, IEntityRepository<T> dal)
        {
            _validator = (IValidator)Activator.CreateInstance(validatorType);
            _dal = dal;
        }

        public IDataResult<List<T>> GetAll()
        {
            throw new System.NotImplementedException();
        }

        public IResult Add(T entity)
        {
            return Process(0, entity);
        }

        public IResult Update(T entity)
        {
            return Process(1, entity);
        }

        public IResult Delete(T entity)
        {
            return Process(2, entity);
        }

        private IResult Process(int i, IEntity entity)
        {
            var validatorResult = ValidationTool.Validate(_validator, entity);
            if (validatorResult.Success)
            {
                switch (i)
                {
                    case 0:
                        return _dal.Add((T)entity);
                    case 1:
                        return _dal.Update((T)entity);
                    case 2:
                        return _dal.Delete((T)entity);
                }
            }
            return new ErrorResult(ErrorMessageBuilder.CreateErrorMessageFromValidationFailure(validatorResult.Data));
        }
    }
}
