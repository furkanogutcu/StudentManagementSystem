using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using StudentManagementSystem.Core.Entities;

namespace StudentManagementSystem.Core.DataAccess.Sql.Utilities
{
    public static class ModelHelper<TEntity>
        where TEntity : class, IEntity, new()
    {
        /// <summary>
        /// This method converts the lines from the MySqlDataReader object it receives into Entity objects that the class works with and returns these objects as a list.
        /// <param name="reader">MySqlDataReader object</param>
        /// <returns>A list containing objects of the given Entity type</returns>
        public static List<TEntity> GetInstanceListFromReader(MySqlDataReader reader)
        {
            List<TEntity> list = new List<TEntity>();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    TEntity entity = new TEntity();
                    var entityProperties = entity.GetType().GetProperties();
                    if (entityProperties.Length != reader.FieldCount)
                        throw new Exception($"The field count of the {entity.GetType().Name} entity does not match the number of fields in the corresponding database table.");

                    for (int i = 0; i < entityProperties.Length; i++)
                    {
                        var data = reader.GetValue(i);

                        if (data is string)
                        {
                            entity.GetType().GetProperties()[i].SetValue(entity, data.ToString());
                        }
                        else if (data is int)
                        {
                            entity.GetType().GetProperties()[i].SetValue(entity, Convert.ToInt32(data));
                        }
                        else if (data is short) //For the 'Year' data type in databases
                        {
                            entity.GetType().GetProperties()[i].SetValue(entity, short.Parse(data.ToString()));
                        }
                        else if (data is double)
                        {
                            entity.GetType().GetProperties()[i].SetValue(entity, Convert.ToDouble(data));
                        }
                        else if (data is DateTime)
                        {
                            entity.GetType().GetProperties()[i].SetValue(entity, Convert.ToDateTime(data));
                        }
                        else if (data is DBNull)
                        {
                            entity.GetType().GetProperties()[i].SetValue(entity, null);
                        }
                    }
                    list.Add(entity);
                }
            }

            return list;
        }
    }
}
