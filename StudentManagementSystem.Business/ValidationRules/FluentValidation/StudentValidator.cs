using System;
using FluentValidation;
using StudentManagementSystem.Entities.Concrete;

namespace StudentManagementSystem.Business.ValidationRules.FluentValidation
{
    public class StudentValidator : AbstractValidator<Student>
    {
        public StudentValidator()
        {
            RuleFor(s => s.StudentNo).NotEmpty();
            RuleFor(s => s.StudentNo).NotNull();
            RuleFor(s => s.StudentNo).GreaterThan(0);

            RuleFor(s => s.DepartmentNo).NotEmpty();
            RuleFor(s => s.DepartmentNo).NotNull();
            RuleFor(s => s.DepartmentNo).GreaterThan(0);

            RuleFor(s => s.AdviserNo).NotEmpty();
            RuleFor(s => s.AdviserNo).NotNull();
            RuleFor(s => s.AdviserNo).GreaterThan(0);

            RuleFor(s => s.Email).NotEmpty();
            RuleFor(s => s.Email).NotNull();
            RuleFor(s => s.Email).EmailAddress();

            RuleFor(s => s.Password).NotEmpty();
            RuleFor(s => s.Password).NotNull();
            RuleFor(s => s.Password).MinimumLength(8);
            RuleFor(s => s.Password).MaximumLength(50);

            RuleFor(s => s.FirstName).NotEmpty();
            RuleFor(s => s.FirstName).NotNull();
            RuleFor(s => s.FirstName).MinimumLength(2);
            RuleFor(s => s.FirstName).MaximumLength(50);

            RuleFor(s => s.LastName).NotEmpty();
            RuleFor(s => s.LastName).NotNull();
            RuleFor(s => s.LastName).MinimumLength(2);
            RuleFor(s => s.LastName).MaximumLength(50);

            RuleFor(s => s.Phone).NotEmpty();
            RuleFor(s => s.Phone).NotNull();
            RuleFor(s => s.Phone).MinimumLength(10);
            RuleFor(s => s.Phone).MaximumLength(15);

            RuleFor(s => s.Semester).NotEmpty();
            RuleFor(s => s.Semester).NotNull();
            RuleFor(s => s.Semester).GreaterThan(0);

            RuleFor(s => s.EnrollmentDate).NotEmpty();
            RuleFor(s => s.EnrollmentDate).NotNull();
            RuleFor(s => s.EnrollmentDate).LessThanOrEqualTo(short.Parse(DateTime.Now.Year.ToString()));
        }
    }
}
