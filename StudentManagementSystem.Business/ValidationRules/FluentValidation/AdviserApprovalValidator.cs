using FluentValidation;
using StudentManagementSystem.Entities.Concrete;

namespace StudentManagementSystem.Business.ValidationRules.FluentValidation
{
    public class AdviserApprovalValidator : AbstractValidator<AdviserApproval>
    {
        public AdviserApprovalValidator()
        {
            RuleFor(aa => aa.Id).NotEmpty();
            RuleFor(aa => aa.Id).NotNull();
            RuleFor(aa => aa.Id).GreaterThan(0);

            RuleFor(aa => aa.StudentNo).NotEmpty().WithName("Öğrenci no");
            RuleFor(aa => aa.StudentNo).NotNull().WithName("Öğrenci no");
            RuleFor(aa => aa.StudentNo).GreaterThan(0).WithName("Öğrenci no");

            RuleFor(aa => aa.CatalogCourseCode).NotEmpty().WithName("Ders kodu");
            RuleFor(aa => aa.CatalogCourseCode).NotNull().WithName("Ders kodu");
            RuleFor(aa => aa.CatalogCourseCode).GreaterThan(0).WithName("Ders kodu");
        }
    }
}
