using System;
using StudentManagementSystem.Core.Entities;

namespace StudentManagementSystem.Entities.Concrete
{
    public class EnrolledCourse : IEntity
    {
        public int Id { get; set; }
        public int CourseNo { get; set; }
        public int StudentNo { get; set; }
        public DateTime EnrolledDate { get; set; }
        public int? VizeResult { get; set; }
        public int? FinalResult { get; set; }
        public int? ButunlemeResult { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
