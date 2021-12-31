using System.Collections.Generic;
using StudentManagementSystem.Core.Entities;
using StudentManagementSystem.Core.Utilities.Results;

namespace StudentManagementSystem.Core.DataAccess.Sql
{
    public interface IEntityRepository<T> where T : class, IEntity, new()
    {
        IDataResult<List<T>> GetAll(Dictionary<string,dynamic>? conditions);
        IDataResult<T> Get(Dictionary<string, dynamic> conditions);
        IResult Add(T entity);
        IResult Update(T entity);
        IResult Delete(T entity);
    }
}
