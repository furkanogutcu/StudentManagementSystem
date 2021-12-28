using System;
using StudentManagementSystem.Core.Entities;

namespace StudentManagementSystem.Entities.Concrete
{
    public class Instructor : IEntity
    {
        public int InstructorNo { get; set; }
        public int DepartmentNo { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
