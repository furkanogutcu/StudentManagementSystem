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

            RuleFor(ec => ec.CourseNo).NotEmpty().WithName("Ders kodu");
            RuleFor(ec => ec.CourseNo).NotNull().WithName("Ders kodu");
            RuleFor(ec => ec.CourseNo).GreaterThan(0).WithName("Ders kodu");

            RuleFor(ec => ec.StudentNo).NotEmpty().WithName("Öğrenci no");
            RuleFor(ec => ec.StudentNo).NotNull().WithName("Öğrenci no");
            RuleFor(ec => ec.StudentNo).GreaterThan(0).WithName("Öğrenci no");

            RuleFor(ec => ec.EnrolledDate).NotEmpty().WithName("Kayıtlanma tarihi");
            RuleFor(ec => ec.EnrolledDate).NotNull().WithName("Kayıtlanma tarihi");

            RuleFor(ec => ec.VizeResult).GreaterThanOrEqualTo(0).When(ec => ec.VizeResult != null).WithName("Vize notu");
            RuleFor(ec => ec.VizeResult).LessThanOrEqualTo(100).When(ec => ec.VizeResult != null).WithName("Vize notu");

            RuleFor(ec => ec.FinalResult).GreaterThanOrEqualTo(0).When(ec => ec.FinalResult != null).WithName("Final notu");
            RuleFor(ec => ec.FinalResult).LessThanOrEqualTo(100).When(ec => ec.FinalResult != null).WithName("Final notu");

            RuleFor(ec => ec.ButunlemeResult).GreaterThanOrEqualTo(0).When(ec => ec.ButunlemeResult != null).WithName("Bütünleme notu");
            RuleFor(ec => ec.ButunlemeResult).LessThanOrEqualTo(100).When(ec => ec.ButunlemeResult != null).WithName("Bütünleme notu");
        }
    }
}
