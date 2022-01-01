using System;
using MySql.Data.MySqlClient;
using StudentManagementSystem.Core.DataAccess.Sql;
using StudentManagementSystem.Core.DataAccess.Sql.Utilities;
using StudentManagementSystem.Core.Utilities.Results;
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

        public override IResult Add(Department entity)
        {
            MySqlConnection connection = ConnectionHelper.OpenConnection();
            try
            {
                MySqlCommand command = new MySqlCommand($"INSERT INTO {GetTableName()}(bolum_adi,donem_sayisi) VALUES (@bolum_adi,@donem_sayisi)", connection);
                command.Parameters.AddWithValue("@bolum_adi", entity.DepartmentName);
                command.Parameters.AddWithValue("@donem_sayisi", entity.NumberOfSemester);
                command.ExecuteNonQuery();
                ConnectionHelper.CloseConnection(connection);
                return new SuccessResult();
            }
            catch (Exception e)
            {
                ConnectionHelper.CloseConnection(connection);
                return new ErrorResult(e.Message);
            }
        }

        public override IResult Update(Department entity)
        {
            MySqlConnection connection = ConnectionHelper.OpenConnection();
            try
            {
                MySqlCommand command = new MySqlCommand($"UPDATE {GetTableName()} SET bolum_adi = @bolum_adi, donem_sayisi = @donem_sayisi, modified_at = @modified_at WHERE bolum_no = @bolum_no", connection);
                command.Parameters.AddWithValue("@bolum_adi", entity.DepartmentName);
                command.Parameters.AddWithValue("@donem_sayisi", entity.NumberOfSemester);
                command.Parameters.AddWithValue("@modified_at", DateTime.Now);
                command.Parameters.AddWithValue("@bolum_no", entity.DepartmentNo);
                command.ExecuteNonQuery();
                ConnectionHelper.CloseConnection(connection);
                return new SuccessResult();
            }
            catch (Exception e)
            {
                ConnectionHelper.CloseConnection(connection);
                return new ErrorResult(e.Message);
            }
        }

        public override IResult Delete(Department entity)
        {
            MySqlConnection connection = ConnectionHelper.OpenConnection();
            try
            {
                MySqlCommand command = new MySqlCommand($"UPDATE {GetTableName()} SET deleted_at = @deleted_at WHERE bolum_no = @bolum_no", connection);
                command.Parameters.AddWithValue("@deleted_at", DateTime.Now);
                command.Parameters.AddWithValue("@bolum_no", entity.DepartmentNo);
                command.ExecuteNonQuery();
                ConnectionHelper.CloseConnection(connection);
                return new SuccessResult();
            }
            catch (Exception e)
            {
                ConnectionHelper.CloseConnection(connection);
                return new ErrorResult(e.Message);
            }
        }
    }
}
