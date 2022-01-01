using System;
using StudentManagementSystem.Core.Entities;

namespace StudentManagementSystem.Entities.Concrete
{
    public class Department : IEntity
    {
        public int DepartmentNo { get; set; }
        public string DepartmentName { get; set; }
        public int NumberOfSemester { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
