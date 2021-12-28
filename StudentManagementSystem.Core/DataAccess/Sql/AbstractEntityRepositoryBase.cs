using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using StudentManagementSystem.Core.DataAccess.Sql.Utilities;
using StudentManagementSystem.Core.Entities;

namespace StudentManagementSystem.Core.DataAccess.Sql
{
    public abstract class AbstractEntityRepositoryBase<TEntity> : IEntityRepository<TEntity>
        where TEntity : class, IEntity, new()
    {
        public abstract string GetTableName();

        //Virtual because it can be overridden in subclasses.
        public virtual List<TEntity> GetAll(string? condition)
        {
            MySqlConnection connection = ConnectionHelper.OpenConnection();
            try
            {
                MySqlCommand command = new MySqlCommand(
                    condition == null
                        ? $"SELECT * FROM {GetTableName()}"
                        : $"SELECT * FROM {GetTableName()} WHERE {condition}",
                    connection);
                MySqlDataReader reader = command.ExecuteReader();
                var result = ModelHelper<TEntity>.GetInstanceListFromReader(reader);
                ConnectionHelper.CloseConnection(connection);
                return result;
            }
            catch (Exception e)
            {
                ConnectionHelper.CloseConnection(connection);
                throw e;
            }
        }

        //Virtual because it can be overridden in subclasses.
        public virtual TEntity Get(string condition)
        {
            MySqlConnection connection = ConnectionHelper.OpenConnection();
            try
            {
                MySqlCommand command = new MySqlCommand($"SELECT * FROM {GetTableName()} WHERE {condition}", connection);
                MySqlDataReader reader = command.ExecuteReader();
                var result = ModelHelper<TEntity>.GetInstanceListFromReader(reader)[0];
                ConnectionHelper.CloseConnection(connection);
                return result;
            }
            catch (Exception e)
            {
                ConnectionHelper.CloseConnection(connection);
                throw e;
            }
        }

        public abstract void Add(TEntity entity);

        public abstract void Update(TEntity entity);

        public abstract void Delete(TEntity entity);
    }
}
