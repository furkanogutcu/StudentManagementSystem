using System;
using System.Windows.Forms;
using StudentManagementSystem.Entities.Concrete;

namespace StudentManagementSystem.Application.Utilities
{
    public static class UniqueValueTaker
    {
        public static int GetUniqueValueOfSelectedItemInListbox<T>(ListBox listBox)
        {
            int uniqueValueOfSelectedIndex;
            // The first item selected in the listbox is of T type. The next selected items are of type int.
            var tempObject = listBox.SelectedItem;

            switch (typeof(T).Name)
            {
                case nameof(Department):
                    uniqueValueOfSelectedIndex = ((Department)tempObject).DepartmentNo;
                    break;
                case nameof(Student):
                    uniqueValueOfSelectedIndex = ((Student)tempObject).StudentNo;
                    break;
                case nameof(Officer):
                    uniqueValueOfSelectedIndex = ((Officer)tempObject).OfficerNo;
                    break;
                case nameof(Instructor):
                    uniqueValueOfSelectedIndex = ((Instructor)tempObject).InstructorNo;
                    break;
                case nameof(EnrolledCourse):
                    uniqueValueOfSelectedIndex = ((EnrolledCourse)tempObject).Id;
                    break;
                case nameof(CatalogCourse):
                    uniqueValueOfSelectedIndex = ((CatalogCourse)tempObject).CourseNo;
                    break;
                case nameof(AdviserApproval):
                    uniqueValueOfSelectedIndex = ((AdviserApproval)tempObject).Id;
                    break;
                default:
                    uniqueValueOfSelectedIndex = Convert.ToInt32(listBox.SelectedItem);
                    break;
            }

            return uniqueValueOfSelectedIndex;
        }

        public static int GetUniqueValueOfSelectedItemInComboBox<T>(ComboBox comboBox)
        {
            int uniqueValueOfSelectedIndex;
            // The first item selected in the listbox is of T type. The next selected items are of type int.
            var tempObject = comboBox.SelectedItem;

            switch (typeof(T).Name)
            {
                case nameof(Department):
                    uniqueValueOfSelectedIndex = ((Department)tempObject).DepartmentNo;
                    break;
                case nameof(Student):
                    uniqueValueOfSelectedIndex = ((Student)tempObject).StudentNo;
                    break;
                case nameof(Officer):
                    uniqueValueOfSelectedIndex = ((Officer)tempObject).OfficerNo;
                    break;
                case nameof(Instructor):
                    uniqueValueOfSelectedIndex = ((Instructor)tempObject).InstructorNo;
                    break;
                case nameof(EnrolledCourse):
                    uniqueValueOfSelectedIndex = ((EnrolledCourse)tempObject).Id;
                    break;
                case nameof(CatalogCourse):
                    uniqueValueOfSelectedIndex = ((CatalogCourse)tempObject).CourseNo;
                    break;
                case nameof(AdviserApproval):
                    uniqueValueOfSelectedIndex = ((AdviserApproval)tempObject).Id;
                    break;
                default:
                    uniqueValueOfSelectedIndex = Convert.ToInt32(comboBox.SelectedItem);
                    break;
            }

            return uniqueValueOfSelectedIndex;
        }
    }
}
