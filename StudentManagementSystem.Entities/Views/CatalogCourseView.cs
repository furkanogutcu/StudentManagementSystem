using StudentManagementSystem.Core.Entities;

namespace StudentManagementSystem.Entities.Views
{
    public class CatalogCourseView : IViews
    {
        public int CourseNo { get; set; }
        public string CourseName { get; set; }
        public string InstructorFullName { get; set; }
        public int Credit { get; set; }
        public int CourseSemester { get; set; }
        public string DepartmentName { get; set; }
    }
}
