using System;
using MySql.Data.MySqlClient;
using StudentManagementSystem.Core.DataAccess.Sql;
using StudentManagementSystem.Core.DataAccess.Sql.Utilities;
using StudentManagementSystem.DataAccess.Abstract;
using StudentManagementSystem.Entities.Concrete;

namespace StudentManagementSystem.DataAccess.Concrete.Sql
{
    public class SqlDepartmentDal : AbstractEntityRepositoryBase<Department>, IDepartmentDal
    {
        public override string GetTableName()
        {
            return "tblbolum";
        }

        public override void Add(Department entity)
        {
            MySqlConnection connection = ConnectionHelper.OpenConnection();
            try
            {
                MySqlCommand command = new MySqlCommand($"INSERT INTO {GetTableName()}(bolum_adi) VALUES (@bolum_adi)", connection);
                command.Parameters.AddWithValue("@bolum_adi", entity.DepartmentName);
                command.ExecuteNonQuery();
                ConnectionHelper.CloseConnection(connection);
            }
            catch (Exception e)
            {
                ConnectionHelper.CloseConnection(connection);
                throw e;
            }
        }

        public override void Update(Department entity)
        {
            MySqlConnection connection = ConnectionHelper.OpenConnection();
            try
            {
                MySqlCommand command = new MySqlCommand($"UPDATE {GetTableName()} SET bolum_adi = @bolum_adi, modified_at = @modified_at WHERE bolum_no = @bolum_no", connection);
                command.Parameters.AddWithValue("@bolum_adi", entity.DepartmentName);
                command.Parameters.AddWithValue("@modified_at", DateTime.Now);
                command.Parameters.AddWithValue("@bolum_no", entity.DepartmentNo);
                command.ExecuteNonQuery();
                ConnectionHelper.CloseConnection(connection);
            }
            catch (Exception e)
            {
                ConnectionHelper.CloseConnection(connection);
                throw e;
            }
        }

        public override void Delete(Department entity)
        {
            MySqlConnection connection = ConnectionHelper.OpenConnection();
            try
            {
                MySqlCommand command = new MySqlCommand($"UPDATE {GetTableName()} SET deleted_at = @deleted_at WHERE bolum_no = @bolum_no", connection);
                command.Parameters.AddWithValue("@deleted_at", DateTime.Now);
                command.Parameters.AddWithValue("@bolum_no", entity.DepartmentNo);
                command.ExecuteNonQuery();
                ConnectionHelper.CloseConnection(connection);
            }
            catch (Exception e)
            {
                ConnectionHelper.CloseConnection(connection);
                throw e;
            }
        }
    }
}
