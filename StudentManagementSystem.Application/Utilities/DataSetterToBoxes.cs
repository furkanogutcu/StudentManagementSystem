using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using StudentManagementSystem.Core.Entities;
using StudentManagementSystem.Entities.Concrete;

namespace StudentManagementSystem.Application.Utilities
{
    public static class DataSetterToBoxes
    {
        public static void SetDataToListBox<T>(ListBox listBox, List<T> dataList)
            where T : class, IEntity, new()
        {
            var finalData = new List<T>();
            foreach (var data in dataList)
            {
                finalData.Add(data);
            }

            listBox.DataSource = finalData;
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

        public static void SetDataToComboBox<T>(ComboBox comboBox, List<T> dataList, Func<T, bool> condition)
            where T : class, IEntity, new()
        {
            var finalData = new List<T>();
            foreach (var data in dataList)
            {
                finalData.Add(data);
            }

            if (condition != null)
            {
                finalData = finalData.Where(condition).ToList();
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

        public static void SetDataToCheckedListBox<T>(CheckedListBox checkedListBox, List<T> dataList)
            where T : class, IEntity, new()
        {
            var finalData = new List<T>();
            foreach (var data in dataList)
            {
                finalData.Add(data);
            }

            checkedListBox.DataSource = finalData;
            switch (typeof(T).Name)
            {
                case nameof(Department):
                    checkedListBox.DisplayMember = nameof(Department.DepartmentName);
                    checkedListBox.ValueMember = nameof(Department.DepartmentNo);
                    break;
                case nameof(Student):
                    checkedListBox.DisplayMember = nameof(Student.FirstName);
                    checkedListBox.ValueMember = nameof(Student.StudentNo);
                    break;
                case nameof(Officer):
                    checkedListBox.DisplayMember = nameof(Officer.FirstName);
                    checkedListBox.ValueMember = nameof(Officer.OfficerNo);
                    break;
                case nameof(Instructor):
                    checkedListBox.DisplayMember = nameof(Instructor.FirstName);
                    checkedListBox.ValueMember = nameof(Instructor.InstructorNo);
                    break;
                case nameof(CatalogCourse):
                    checkedListBox.DisplayMember = nameof(CatalogCourse.CourseName);
                    checkedListBox.ValueMember = nameof(CatalogCourse.CourseNo);
                    break;
                case nameof(AdviserApproval):
                    checkedListBox.DisplayMember = nameof(AdviserApproval.CatalogCourseCode);
                    checkedListBox.ValueMember = nameof(AdviserApproval.CatalogCourseCode);
                    break;
                default:
                    throw new Exception("Tip eşleme hatası");
            }
        }

        public static void SetComboBoxSelectedItem<T>(ComboBox comboBox, string primaryField)
            where T : class, IEntity, new()
        {
            for (int i = 0; i < comboBox.Items.Count; i++)
            {
                var selectedItem = comboBox.Items[i];

                bool condition;

                switch (typeof(T).Name)
                {
                    case nameof(Department):
                        condition = ((Department)selectedItem).DepartmentNo == Convert.ToInt32(primaryField);
                        break;
                    case nameof(Instructor):
                        condition = ((Instructor)selectedItem).InstructorNo == Convert.ToInt32(primaryField);
                        break;
                    default:
                        condition = false;
                        break;
                }

                if (condition)
                {
                    comboBox.SelectedItem = comboBox.Items[i];
                    break;
                }
            }
        }
    }
}
