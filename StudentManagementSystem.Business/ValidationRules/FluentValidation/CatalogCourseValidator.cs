using System;
using FluentValidation;
using StudentManagementSystem.Entities.Concrete;

namespace StudentManagementSystem.Business.ValidationRules.FluentValidation
{
    public class CatalogCourseValidator : AbstractValidator<CatalogCourse>
    {
        public CatalogCourseValidator()
        {
            RuleFor(cc => cc.CourseNo).NotEmpty().WithName("Ders kodu");
            RuleFor(cc => cc.CourseNo).NotNull().WithName("Ders kodu");
            RuleFor(cc => cc.CourseNo).GreaterThan(0).WithName("Ders kodu");

            RuleFor(cc => cc.DepartmentNo).NotEmpty().WithName("Bölüm kodu");
            RuleFor(cc => cc.DepartmentNo).NotNull().WithName("Bölüm kodu");
            RuleFor(cc => cc.DepartmentNo).GreaterThan(0).WithName("Bölüm kodu");

            RuleFor(cc => cc.InstructorNo).NotEmpty().WithName("Öğretim görevlisi no");
            RuleFor(cc => cc.InstructorNo).NotNull().WithName("Öğretim görevlisi no");
            RuleFor(cc => cc.InstructorNo).GreaterThan(0).WithName("Öğretim görevlisi no");

            RuleFor(cc => cc.CourseName).NotEmpty().WithName("Ders adı");
            RuleFor(cc => cc.CourseName).NotNull().WithName("Ders adı");
            RuleFor(cc => cc.CourseName).MinimumLength(2).WithName("Ders adı");
            RuleFor(cc => cc.CourseName).MaximumLength(100).WithName("Ders adı");

            RuleFor(cc => cc.Credit).NotEmpty().WithName("Kredi");
            RuleFor(cc => cc.Credit).NotNull().WithName("Kredi");
            RuleFor(cc => cc.Credit).GreaterThan(0).WithName("Kredi");

            RuleFor(cc => cc.CourseSemester).NotEmpty().WithName("Ders dönemi");
            RuleFor(cc => cc.CourseSemester).NotNull().WithName("Ders dönemi");
            RuleFor(cc => cc.CourseSemester).GreaterThan(0).WithName("Ders dönemi");
        }
    }
}
