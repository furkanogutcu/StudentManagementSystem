using System;
using StudentManagementSystem.Core.Entities;

namespace StudentManagementSystem.Entities.Concrete
{
    public class CatalogCourse : IEntity
    {
        public int CourseNo { get; set; }
        public int DepartmentNo { get; set; }
        public int InstructorNo { get; set; }
        public String CourseName { get; set; }
        public int Credit { get; set; }
        public short CourseYear { get; set; }
        public int CourseSemester { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
