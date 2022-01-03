using System;
using StudentManagementSystem.Core.Entities;

namespace StudentManagementSystem.Entities.Views
{
    public class EnrolledCourseView : IViews
    {
        public int CourseNo { get; set; }
        public string CourseName { get; set; }
        public string InstructorFullName { get; set; }
        public int? VizeResult { get; set; }
        public int? FinalResult { get; set; }
        public int? ButunlemeResult { get; set; }

        private int? CompletionGrade { get; }
        private bool? Status { get; }

        public int? GetCompletionGrade()
        {
            if (VizeResult != null && FinalResult != null && ButunlemeResult == null)
            {
                return Convert.ToInt32((VizeResult * 0.6) + (FinalResult * 0.4));
            }

            if (VizeResult != null && FinalResult != null && ButunlemeResult != null)
            {
                return Convert.ToInt32((VizeResult * 0.6) + ((FinalResult >= ButunlemeResult ? FinalResult : ButunlemeResult) * 0.4));
            }
            return null;
        }

        public bool? GetStatus()
        {
            if (GetCompletionGrade() != null)
            {
                return GetCompletionGrade() > 50;
            }
            return null;
        }
    }
}
