using System;
using MySql.Data.MySqlClient;
using StudentManagementSystem.Core.DataAccess.Sql;
using StudentManagementSystem.Core.DataAccess.Sql.Utilities;
using StudentManagementSystem.Core.Utilities.Results;
using StudentManagementSystem.DataAccess.Abstract;
using StudentManagementSystem.Entities.Concrete;

namespace StudentManagementSystem.DataAccess.Concrete.Sql
{
    public class SqlInstructorDal : AbstractEntityRepositoryBase<Instructor>, IInstructorDal
    {
        public override string GetTableName()
        {
            return "tblogretimuyesi";
        }

        public override IResult Add(Instructor entity)
        {
            MySqlConnection connection = ConnectionHelper.OpenConnection();
            try
            {
                MySqlCommand command = new MySqlCommand($"INSERT INTO {GetTableName()}(bolum_no,email,sifre,ad,soyad,telefon) VALUES (@bolum_no,@email,@sifre,@ad,@soyad,@telefon)", connection);
                command.Parameters.AddWithValue("@bolum_no", entity.DepartmentNo);
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

        public override IResult Update(Instructor entity)
        {
            MySqlConnection connection = ConnectionHelper.OpenConnection();
            try
            {
                MySqlCommand command = new MySqlCommand(
                    $"UPDATE {GetTableName()} SET bolum_no = @bolum_no, email = @email, sifre = @sifre, ad = @ad, soyad = @soyad, telefon = @telefon WHERE ogretim_uye_no = @ogretim_uye_no",
                    connection);
                command.Parameters.AddWithValue("@bolum_no", entity.DepartmentNo);
                command.Parameters.AddWithValue("@email", entity.Email);
                command.Parameters.AddWithValue("@sifre", entity.Password);
                command.Parameters.AddWithValue("@ad", entity.FirstName);
                command.Parameters.AddWithValue("@soyad", entity.LastName);
                command.Parameters.AddWithValue("@telefon", entity.Phone);
                command.Parameters.AddWithValue("@ogretim_uye_no", entity.InstructorNo);
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

        public override IResult Delete(Instructor entity)
        {
            MySqlConnection connection = ConnectionHelper.OpenConnection();
            try
            {
                MySqlCommand command = new MySqlCommand($"UPDATE {GetTableName()} SET deleted_at = @deleted_at WHERE ogretim_uye_no = @ogretim_uye_no", connection);
                command.Parameters.AddWithValue("@deleted_at", DateTime.Now);
                command.Parameters.AddWithValue("@ogretim_uye_no", entity.InstructorNo);
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
