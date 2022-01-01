using System.Collections.Generic;
using StudentManagementSystem.Core.Entities;
using StudentManagementSystem.Core.Utilities.Results;

namespace StudentManagementSystem.Business.Abstract
{
    public interface IEntityCrudService<T>
        where T : class, IEntity, new()
    {
        IDataResult<List<T>> GetAll();
        IResult Add(T entity);
        IResult Update(T entity);
        IResult Delete(T entity);
    }
}
