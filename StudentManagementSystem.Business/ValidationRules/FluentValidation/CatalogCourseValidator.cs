using System;
using FluentValidation;
using StudentManagementSystem.Entities.Concrete;

namespace StudentManagementSystem.Business.ValidationRules.FluentValidation
{
    public class CatalogCourseValidator : AbstractValidator<CatalogCourse>
    {
        public CatalogCourseValidator()
        {
            RuleFor(cc => cc.CourseNo).NotEmpty();
            RuleFor(cc => cc.CourseNo).NotNull();
            RuleFor(cc => cc.CourseNo).GreaterThan(0);

            RuleFor(cc => cc.DepartmentNo).NotEmpty();
            RuleFor(cc => cc.DepartmentNo).NotNull();
            RuleFor(cc => cc.DepartmentNo).GreaterThan(0);

            RuleFor(cc => cc.InstructorNo).NotEmpty();
            RuleFor(cc => cc.InstructorNo).NotNull();
            RuleFor(cc => cc.InstructorNo).GreaterThan(0);

            RuleFor(cc => cc.CourseName).NotEmpty();
            RuleFor(cc => cc.CourseName).NotNull();
            RuleFor(cc => cc.CourseName).MinimumLength(2);
            RuleFor(cc => cc.CourseName).MaximumLength(100);

            RuleFor(cc => cc.Credit).NotEmpty();
            RuleFor(cc => cc.Credit).NotNull();
            RuleFor(cc => cc.Credit).GreaterThan(0);

            RuleFor(cc => cc.CourseYear).NotEmpty();
            RuleFor(cc => cc.CourseYear).NotNull();
            RuleFor(cc => cc.CourseYear).GreaterThanOrEqualTo(short.Parse(DateTime.Now.Year.ToString()));

            RuleFor(cc => cc.CourseSemester).NotEmpty();
            RuleFor(cc => cc.CourseSemester).NotNull();
            RuleFor(cc => cc.CourseSemester).GreaterThan(0);
        }
    }
}
