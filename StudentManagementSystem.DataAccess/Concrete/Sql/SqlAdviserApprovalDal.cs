using System;
using MySql.Data.MySqlClient;
using StudentManagementSystem.Core.DataAccess.Sql;
using StudentManagementSystem.Core.DataAccess.Sql.Utilities;
using StudentManagementSystem.Core.Utilities.Results;
using StudentManagementSystem.DataAccess.Abstract;
using StudentManagementSystem.Entities.Concrete;

namespace StudentManagementSystem.DataAccess.Concrete.Sql
{
    public class SqlAdviserApprovalDal : AbstractEntityRepositoryBase<AdviserApproval>,IAdviserApprovalDal
    {
        public override string GetTableName()
        {
            return "tbldanismanonay";
        }

        public override IResult Add(AdviserApproval entity)
        {
            MySqlConnection connection = ConnectionHelper.OpenConnection();
            try
            {
                MySqlCommand command = new MySqlCommand($"INSERT INTO {GetTableName()}(ogrenci_no,katalog_ders_kodu) VALUES (@ogrenci_no,@katalog_ders_kodu)", connection);
                command.Parameters.AddWithValue("@ogrenci_no", entity.StudentNo);
                command.Parameters.AddWithValue("@katalog_ders_kodu", entity.CatalogCourseCode);
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

        public override IResult Update(AdviserApproval entity)
        {
            MySqlConnection connection = ConnectionHelper.OpenConnection();
            try
            {
                MySqlCommand command = new MySqlCommand($"UPDATE {GetTableName()} SET ogrenci_no = @ogrenci_no, katalog_ders_kodu = @katalog_ders_kodu, modified_at = @modified_at WHERE id = @id", connection);
                command.Parameters.AddWithValue("@ogrenci_no", entity.StudentNo);
                command.Parameters.AddWithValue("@katalog_ders_kodu", entity.CatalogCourseCode);
                command.Parameters.AddWithValue("@modified_at", DateTime.Now);
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

        public override IResult Delete(AdviserApproval entity)
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
