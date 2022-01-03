using System;
using StudentManagementSystem.Core.Entities;

namespace StudentManagementSystem.Entities.Concrete
{
    public class AdviserApproval : IEntity
    {
        public int Id { get; set; }
        public int StudentNo { get; set; }
        public int CatalogCourseCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
