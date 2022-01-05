using System;
using MySql.Data.MySqlClient;
using StudentManagementSystem.Core.DataAccess.Sql;
using StudentManagementSystem.Core.DataAccess.Sql.Utilities;
using StudentManagementSystem.Core.Utilities.Results;
using StudentManagementSystem.DataAccess.Abstract;
using StudentManagementSystem.Entities.Concrete;

namespace StudentManagementSystem.DataAccess.Concrete.Sql
{
    public class SqlStudentDal : AbstractEntityRepositoryBase<Student>, IStudentDal
    {
        public override string GetTableName()
        {
            return "tblogrenci";
        }

        public override IResult Add(Student entity)
        {
            MySqlConnection connection = ConnectionHelper.OpenConnection();
            try
            {
                MySqlCommand command = new MySqlCommand($"INSERT INTO {GetTableName()}(bolum_no,danisman_no,email,sifre,ad,soyad,telefon,donem,kayit_yili) VALUES (@bolum_no,@danisman_no,@email,@sifre,@ad,@soyad,@telefon,@donem,@kayit_yili)", connection);
                command.Parameters.AddWithValue("@bolum_no", entity.DepartmentNo);
                command.Parameters.AddWithValue("@danisman_no", entity.AdviserNo);
                command.Parameters.AddWithValue("@email", entity.Email);
                command.Parameters.AddWithValue("@sifre", entity.Password);
                command.Parameters.AddWithValue("@ad", entity.FirstName);
                command.Parameters.AddWithValue("@soyad", entity.LastName);
                command.Parameters.AddWithValue("@telefon", entity.Phone);
                command.Parameters.AddWithValue("@donem", 1);
                command.Parameters.AddWithValue("@kayit_yili", short.Parse(DateTime.Now.Year.ToString()));
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

        public override IResult Update(Student entity)
        {
            MySqlConnection connection = ConnectionHelper.OpenConnection();
            try
            {
                MySqlCommand command = new MySqlCommand($"UPDATE {GetTableName()} SET bolum_no = @bolum_no, danisman_no = @danisman_no, email = @email, sifre = @sifre, ad = @ad, soyad = @soyad, telefon = @telefon, donem = @donem, kayit_yili = @kayit_yili, modified_at = @modified_at WHERE ogrenci_no = @ogrenci_no", connection);
                command.Parameters.AddWithValue("@bolum_no", entity.DepartmentNo);
                command.Parameters.AddWithValue("@danisman_no", entity.AdviserNo);
                command.Parameters.AddWithValue("@email", entity.Email);
                command.Parameters.AddWithValue("@sifre", entity.Password);
                command.Parameters.AddWithValue("@ad", entity.FirstName);
                command.Parameters.AddWithValue("@soyad", entity.LastName);
                command.Parameters.AddWithValue("@telefon", entity.Phone);
                command.Parameters.AddWithValue("@donem", entity.Semester);
                command.Parameters.AddWithValue("@kayit_yili", entity.EnrollmentDate);
                command.Parameters.AddWithValue("@modified_at", DateTime.Now);
                command.Parameters.AddWithValue("@ogrenci_no", entity.StudentNo);
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

        public override IResult Delete(Student entity)
        {
            MySqlConnection connection = ConnectionHelper.OpenConnection();
            try
            {
                MySqlCommand command = new MySqlCommand($"UPDATE {GetTableName()} SET deleted_at = @deleted_at WHERE ogrenci_no = @ogrenci_no", connection);
                command.Parameters.AddWithValue("@deleted_at", DateTime.Now);
                command.Parameters.AddWithValue("@ogrenci_no", entity.StudentNo);
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
