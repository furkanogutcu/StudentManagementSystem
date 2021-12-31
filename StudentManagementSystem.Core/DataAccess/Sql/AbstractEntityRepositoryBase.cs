using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using StudentManagementSystem.Core.DataAccess.Sql.Utilities;
using StudentManagementSystem.Core.Entities;
using StudentManagementSystem.Core.Utilities.Results;

namespace StudentManagementSystem.Core.DataAccess.Sql
{
    public abstract class AbstractEntityRepositoryBase<TEntity> : IEntityRepository<TEntity>
        where TEntity : class, IEntity, new()
    {
        public abstract string GetTableName();

        //Virtual because it can be overridden in subclasses.
        public virtual IDataResult<List<TEntity>> GetAll(Dictionary<string, dynamic>? conditions)
        {
            MySqlConnection connection = ConnectionHelper.OpenConnection();
            try
            {
                string commandText = $"SELECT * FROM {GetTableName()}";

                if (conditions != null)
                {
                    commandText += " WHERE ";
                    for (int i = 0; i < conditions.Count; i++)
                    {
                        commandText += $"{conditions.ElementAt(i).Key}=@{conditions.ElementAt(i).Key}";
                        if (i != conditions.Count - 1)
                        {
                            commandText += " AND ";
                        }
                    }
                }

                MySqlCommand command = new MySqlCommand(commandText, connection);

                if (conditions != null)
                {
                    foreach (var condition in conditions)
                    {
                        command.Parameters.AddWithValue($"@{condition.Key}", condition.Value);
                    }
                }

                MySqlDataReader reader = command.ExecuteReader();
                var result = ModelHelper<TEntity>.GetInstanceListFromReader(reader);
                ConnectionHelper.CloseConnection(connection);
                return new SuccessDataResult<List<TEntity>>(result);
            }
            catch (Exception e)
            {
                ConnectionHelper.CloseConnection(connection);
                return new ErrorDataResult<List<TEntity>>(e.Message);
            }
        }

        //Virtual because it can be overridden in subclasses.
        public virtual IDataResult<TEntity> Get(Dictionary<string, dynamic> conditions)
        {
            MySqlConnection connection = ConnectionHelper.OpenConnection();
            try
            {
                string commandText = $"SELECT * FROM {GetTableName()} WHERE ";

                for (int i = 0; i < conditions.Count; i++)
                {
                    commandText += $"{conditions.ElementAt(i).Key}=@{conditions.ElementAt(i).Key}";
                    if (i != conditions.Count - 1)
                    {
                        commandText += " AND ";
                    }
                }

                MySqlCommand command = new MySqlCommand(commandText, connection);

                foreach (var condition in conditions)
                {
                    command.Parameters.AddWithValue($"@{condition.Key}", condition.Value);
                }

                MySqlDataReader reader = command.ExecuteReader();
                var result = ModelHelper<TEntity>.GetInstanceListFromReader(reader)[0];
                ConnectionHelper.CloseConnection(connection);
                return new SuccessDataResult<TEntity>(result);
            }
            catch (Exception e)
            {
                ConnectionHelper.CloseConnection(connection);
                return new ErrorDataResult<TEntity>(e.Message);
            }
        }

        public abstract IResult Add(TEntity entity);

        public abstract IResult Update(TEntity entity);

        public abstract IResult Delete(TEntity entity);
    }
}
