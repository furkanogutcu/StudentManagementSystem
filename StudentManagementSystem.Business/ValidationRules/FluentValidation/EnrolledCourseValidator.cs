using System;
using FluentValidation;
using StudentManagementSystem.Entities.Concrete;

namespace StudentManagementSystem.Business.ValidationRules.FluentValidation
{
    public class EnrolledCourseValidator : AbstractValidator<EnrolledCourse>
    {
        public EnrolledCourseValidator()
        {
            RuleFor(ec => ec.Id).NotEmpty();
            RuleFor(ec => ec.Id).NotNull();
            RuleFor(ec => ec.Id).GreaterThan(0);

            RuleFor(ec => ec.CourseNo).NotEmpty();
            RuleFor(ec => ec.CourseNo).NotNull();
            RuleFor(ec => ec.CourseNo).GreaterThan(0);

            RuleFor(ec => ec.StudentNo).NotEmpty();
            RuleFor(ec => ec.StudentNo).NotNull();
            RuleFor(ec => ec.StudentNo).GreaterThan(0);

            RuleFor(ec => ec.EnrolledDate).NotEmpty();
            RuleFor(ec => ec.EnrolledDate).NotNull();
            RuleFor(ec => ec.EnrolledDate).LessThanOrEqualTo(DateTime.Now);

            RuleFor(ec => ec.VizeResult).GreaterThanOrEqualTo(0).When(ec => ec.VizeResult != null);
            RuleFor(ec => ec.VizeResult).LessThanOrEqualTo(100).When(ec => ec.VizeResult != null);

            RuleFor(ec => ec.FinalResult).GreaterThanOrEqualTo(0).When(ec => ec.FinalResult != null);
            RuleFor(ec => ec.FinalResult).LessThanOrEqualTo(100).When(ec => ec.FinalResult != null);

            RuleFor(ec => ec.ButunlemeResult).GreaterThanOrEqualTo(0).When(ec => ec.ButunlemeResult != null);
            RuleFor(ec => ec.ButunlemeResult).LessThanOrEqualTo(100).When(ec => ec.ButunlemeResult != null);
        }
    }
}
