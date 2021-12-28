﻿using System;
using MySql.Data.MySqlClient;
using StudentManagementSystem.Core.DataAccess.Sql;
using StudentManagementSystem.Core.DataAccess.Sql.Utilities;
using StudentManagementSystem.DataAccess.Abstract;
using StudentManagementSystem.Entities.Concrete;

namespace StudentManagementSystem.DataAccess.Concrete.Sql
{
    public class SqlCatalogCourseDal : AbstractEntityRepositoryBase<CatalogCourse>, ICatalogCourseDal
    {
        public override string GetTableName()
        {
            return "tblkatalogders";
        }

        public override void Add(CatalogCourse entity)
        {
            MySqlConnection connection = ConnectionHelper.OpenConnection();
            try
            {
                MySqlCommand command = new MySqlCommand($"INSERT INTO {GetTableName()}(bolum_no,ogretim_uyesi_no,ders_adi,kredi,ders_yili,ders_donemi) VALUES (@bolum_no,@ogretim_uyesi_no,@ders_adi,@kredi,@ders_yili,@ders_donemi)", connection);
                command.Parameters.AddWithValue("@bolum_no", entity.DepartmentNo);
                command.Parameters.AddWithValue("@ogretim_uyesi_no", entity.InstructorNo);
                command.Parameters.AddWithValue("@ders_adi", entity.CourseName);
                command.Parameters.AddWithValue("@kredi", entity.Credit);
                command.Parameters.AddWithValue("@ders_yili", entity.CourseYear);
                command.Parameters.AddWithValue("@ders_donemi", entity.CourseSemester);
                command.ExecuteNonQuery();
                ConnectionHelper.CloseConnection(connection);
            }
            catch (Exception e)
            {
                ConnectionHelper.CloseConnection(connection);
                throw e;
            }
        }

        public override void Update(CatalogCourse entity)
        {
            MySqlConnection connection = ConnectionHelper.OpenConnection();
            try
            {
                MySqlCommand command = new MySqlCommand($"UPDATE {GetTableName()} SET bolum_no = @bolum_no, ogretim_uyesi_no = @ogretim_uyesi_no, ders_adi = @ders_adi, kredi = @kredi, ders_yili = @ders_yili, ders_donemi = @ders_donemi, modified_at = @modified_at WHERE ders_no = @ders_no", connection);
                command.Parameters.AddWithValue("@bolum_no", entity.DepartmentNo);
                command.Parameters.AddWithValue("@ogretim_uyesi_no", entity.InstructorNo);
                command.Parameters.AddWithValue("@ders_adi", entity.CourseName);
                command.Parameters.AddWithValue("@kredi", entity.Credit);
                command.Parameters.AddWithValue("@ders_yili", entity.CourseYear);
                command.Parameters.AddWithValue("@ders_donemi", entity.CourseSemester);
                command.Parameters.AddWithValue("@modified_at", DateTime.Now);
                command.Parameters.AddWithValue("@ders_no", entity.CourseNo);
                command.ExecuteNonQuery();
                ConnectionHelper.CloseConnection(connection);
            }
            catch (Exception e)
            {
                ConnectionHelper.CloseConnection(connection);
                throw e;
            }
        }

        public override void Delete(CatalogCourse entity)
        {
            MySqlConnection connection = ConnectionHelper.OpenConnection();
            try
            {
                MySqlCommand command = new MySqlCommand($"UPDATE {GetTableName()} SET deleted_at = @deleted_at WHERE ders_no = @ders_no", connection);
                command.Parameters.AddWithValue("@deleted_at", DateTime.Now);
                command.Parameters.AddWithValue("@ders_no", entity.CourseNo);
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