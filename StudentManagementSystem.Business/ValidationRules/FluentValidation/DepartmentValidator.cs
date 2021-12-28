using FluentValidation;
using StudentManagementSystem.Entities.Concrete;

namespace StudentManagementSystem.Business.ValidationRules.FluentValidation
{
    public class DepartmentValidator : AbstractValidator<Department>
    {
        public DepartmentValidator()
        {
            RuleFor(d => d.DepartmentNo).NotEmpty();
            RuleFor(d => d.DepartmentNo).NotNull();
            RuleFor(d => d.DepartmentNo).GreaterThan(0);

            RuleFor(d => d.DepartmentName).NotEmpty();
            RuleFor(d => d.DepartmentName).NotNull();
            RuleFor(d => d.DepartmentName).MinimumLength(2);
            RuleFor(d => d.DepartmentName).MaximumLength(200);
        }
    }
}
