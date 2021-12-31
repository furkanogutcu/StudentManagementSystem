using System;
using MySql.Data.MySqlClient;
using StudentManagementSystem.Core.DataAccess.Sql;
using StudentManagementSystem.Core.DataAccess.Sql.Utilities;
using StudentManagementSystem.Core.Utilities.Results;
using StudentManagementSystem.DataAccess.Abstract;
using StudentManagementSystem.Entities.Concrete;

namespace StudentManagementSystem.DataAccess.Concrete.Sql
{
    public class SqlOfficerDal : AbstractEntityRepositoryBase<Officer>, IOfficerDal
    {
        public override string GetTableName()
        {
            return "tblmemur";
        }

        public override IResult Add(Officer entity)
        {
            MySqlConnection connection = ConnectionHelper.OpenConnection();
            try
            {
                MySqlCommand command = new MySqlCommand($"INSERT INTO {GetTableName()}(email,sifre,ad,soyad,telefon) VALUES (@email,@sifre,@ad,@soyad,@telefon)", connection);
                command.Parameters.AddWithValue("@email", entity.Email);
                command.Parameters.AddWithValue("@sifre", entity.Password);
                command.Parameters.AddWithValue("@ad", entity.FirstName);
                command.Parameters.AddWithValue("@soyad", entity.LastName);
                command.Parameters.AddWithValue("@telefon", entity.Phone);
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

        public override IResult Update(Officer entity)
        {
            MySqlConnection connection = ConnectionHelper.OpenConnection();
            try
            {
                MySqlCommand command = new MySqlCommand($"UPDATE {GetTableName()} SET email = @email, sifre = @sifre, ad = @ad, soyad = @soyad, telefon = @telefon, modified_at = @modified_at WHERE memur_no = @memur_no", connection);
                command.Parameters.AddWithValue("@email", entity.Email);
                command.Parameters.AddWithValue("@sifre", entity.Password);
                command.Parameters.AddWithValue("@ad", entity.FirstName);
                command.Parameters.AddWithValue("@soyad", entity.LastName);
                command.Parameters.AddWithValue("@telefon", entity.Phone);
                command.Parameters.AddWithValue("@modified_at", DateTime.Now);
                command.Parameters.AddWithValue("@memur_no", entity.OfficerNo);
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

        public override IResult Delete(Officer entity)
        {
            MySqlConnection connection = ConnectionHelper.OpenConnection();
            try
            {
                MySqlCommand command = new MySqlCommand($"UPDATE {GetTableName()} SET deleted_at = @deleted_at WHERE memur_no = @memur_no", connection);
                command.Parameters.AddWithValue("@deleted_at", DateTime.Now);
                command.Parameters.AddWithValue("@memur_no", entity.OfficerNo);
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
