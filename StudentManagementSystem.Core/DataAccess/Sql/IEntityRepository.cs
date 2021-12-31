using System.Collections.Generic;
using StudentManagementSystem.Core.Entities;

namespace StudentManagementSystem.Core.DataAccess.Sql
{
    public interface IEntityRepository<T> where T : class, IEntity, new()
    {
        List<T> GetAll(Dictionary<string,dynamic>? conditions);
        T Get(Dictionary<string, dynamic> conditions);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
