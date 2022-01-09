using System;
using MySql.Data.MySqlClient;
using StudentManagementSystem.Core.DataAccess.Sql;
using StudentManagementSystem.Core.DataAccess.Sql.Utilities;
using StudentManagementSystem.Core.Utilities.Results;
using StudentManagementSystem.DataAccess.Abstract;
using StudentManagementSystem.Entities.Concrete;

namespace StudentManagementSystem.DataAccess.Concrete.Sql
{
    public class SqlEnrolledCourseDal : AbstractEntityRepositoryBase<EnrolledCourse>, IEnrolledCourseDal
    {
        public override string GetTableName()
        {
            return "tblalinanders";
        }

        public override IResult Add(EnrolledCourse entity)
        {
            MySqlConnection connection = ConnectionHelper.OpenConnection();
            try
            {
                MySqlCommand command = new MySqlCommand($"INSERT INTO {GetTableName()}(ders_no,ogrenci_no,vize_sonuc,final_sonuc,butunleme_sonuc) VALUES (@ders_no,@ogrenci_no,@vize_sonuc,@final_sonuc,@butunleme_sonuc)", connection);
                command.Parameters.AddWithValue("@ders_no", entity.CourseNo);
                command.Parameters.AddWithValue("@ogrenci_no", entity.StudentNo);
                command.Parameters.AddWithValue("@vize_sonuc", entity.VizeResult);
                command.Parameters.AddWithValue("@final_sonuc", entity.FinalResult);
                command.Parameters.AddWithValue("@butunleme_sonuc", entity.ButunlemeResult);
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

        public override IResult Update(EnrolledCourse entity)
        {
            MySqlConnection connection = ConnectionHelper.OpenConnection();
            try
            {
                MySqlCommand command = new MySqlCommand($"UPDATE {GetTableName()} SET ders_no = @ders_no, ogrenci_no = @ogrenci_no, vize_sonuc = @vize_sonuc, final_sonuc = @final_sonuc, butunleme_sonuc = @butunleme_sonuc WHERE id = @id", connection);
                command.Parameters.AddWithValue("@ders_no", entity.CourseNo);
                command.Parameters.AddWithValue("@ogrenci_no", entity.StudentNo);
                command.Parameters.AddWithValue("@vize_sonuc", entity.VizeResult);
                command.Parameters.AddWithValue("@final_sonuc", entity.FinalResult);
                command.Parameters.AddWithValue("@butunleme_sonuc", entity.ButunlemeResult);
                command.Parameters.AddWithValue("@id", entity.Id);
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

        public override IResult Delete(EnrolledCourse entity)
        {
            MySqlConnection connection = ConnectionHelper.OpenConnection();
            try
            {
                MySqlCommand command = new MySqlCommand($"UPDATE {GetTableName()} SET deleted_at = @deleted_at WHERE id = @id", connection);
                command.Parameters.AddWithValue("@deleted_at", DateTime.Now);
                command.Parameters.AddWithValue("@id", entity.Id);
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
