using System;
using System.Collections.Generic;
using System.Windows.Forms;
using StudentManagementSystem.Core.Entities;
using StudentManagementSystem.Entities.Concrete;

namespace StudentManagementSystem.Application.Utilities
{
    public static class DataSetterToBoxes
    {
        public static void SetDataToListBox<T>(ListBox listBox, List<T> data)
            where T : class, IEntity, new()
        {
            listBox.DataSource = data;
            switch (typeof(T).Name)
            {
                case nameof(Department):
                    listBox.DisplayMember = nameof(Department.DepartmentName);
                    listBox.ValueMember = nameof(Department.DepartmentNo);
                    break;
                case nameof(Student):
                    listBox.DisplayMember = nameof(Student.FirstName);
                    listBox.ValueMember = nameof(Student.StudentNo);
                    break;
                case nameof(Officer):
                    listBox.DisplayMember = nameof(Officer.FirstName);
                    listBox.ValueMember = nameof(Officer.OfficerNo);
                    break;
                case nameof(Instructor):
                    listBox.DisplayMember = nameof(Instructor.FirstName);
                    listBox.ValueMember = nameof(Instructor.InstructorNo);
                    break;
                case nameof(CatalogCourse):
                    listBox.DisplayMember = nameof(CatalogCourse.CourseName);
                    listBox.ValueMember = nameof(CatalogCourse.CourseNo);
                    break;
                default:
                    throw new Exception("Tip eşleme hatası");
            }
        }

        public static void SetDataToComboBox<T>(ComboBox comboBox, List<T> dataList)
            where T : class, IEntity, new()
        {
            var finalData = new List<T>();
            foreach (var data in dataList)
            {
                finalData.Add(data);
            }

            comboBox.DataSource = finalData;
            switch (typeof(T).Name)
            {
                case nameof(Department):
                    comboBox.DisplayMember = nameof(Department.DepartmentName);
                    comboBox.ValueMember = nameof(Department.DepartmentNo);
                    break;
                case nameof(Student):
                    comboBox.DisplayMember = nameof(Student.FirstName);
                    comboBox.ValueMember = nameof(Student.StudentNo);
                    break;
                case nameof(Officer):
                    comboBox.DisplayMember = nameof(Officer.FirstName);
                    comboBox.ValueMember = nameof(Officer.OfficerNo);
                    break;
                case nameof(Instructor):
                    comboBox.DisplayMember = nameof(Instructor.FirstName);
                    comboBox.ValueMember = nameof(Instructor.InstructorNo);
                    break;
                case nameof(CatalogCourse):
                    comboBox.DisplayMember = nameof(CatalogCourse.CourseName);
                    comboBox.ValueMember = nameof(CatalogCourse.CourseNo);
                    break;
                default:
                    throw new Exception("Tip eşleme hatası");
            }
        }
    }
}
