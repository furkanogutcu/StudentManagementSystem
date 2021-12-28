using System.Collections.Generic;
using StudentManagementSystem.Core.Entities;

namespace StudentManagementSystem.Core.DataAccess.Sql
{
    public interface IEntityRepository<T> where T : class, IEntity, new()
    {
        List<T> GetAll(string? condition);
        T Get(string condition);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
