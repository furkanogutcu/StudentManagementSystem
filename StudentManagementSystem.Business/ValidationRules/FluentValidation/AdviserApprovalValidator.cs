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

            RuleFor(aa => aa.StudentNo).NotEmpty();
            RuleFor(aa => aa.StudentNo).NotNull();
            RuleFor(aa => aa.StudentNo).GreaterThan(0);

            RuleFor(aa => aa.CatalogCourseCode).NotEmpty();
            RuleFor(aa => aa.CatalogCourseCode).NotNull();
            RuleFor(aa => aa.CatalogCourseCode).GreaterThan(0);
        }
    }
}
