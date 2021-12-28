using FluentValidation;
using StudentManagementSystem.Entities.Concrete;

namespace StudentManagementSystem.Business.ValidationRules.FluentValidation
{
    public class OfficerValidator : AbstractValidator<Officer>
    {
        public OfficerValidator()
        {
            RuleFor(o => o.OfficerNo).NotEmpty();
            RuleFor(o => o.OfficerNo).NotNull();
            RuleFor(o => o.OfficerNo).GreaterThan(0);

            RuleFor(o => o.Email).NotEmpty();
            RuleFor(o => o.Email).NotNull();
            RuleFor(o => o.Email).EmailAddress();

            RuleFor(o => o.Password).NotEmpty();
            RuleFor(o => o.Password).NotNull();
            RuleFor(o => o.Password).MinimumLength(8);
            RuleFor(o => o.Password).MaximumLength(50);

            RuleFor(o => o.FirstName).NotEmpty();
            RuleFor(o => o.FirstName).NotNull();
            RuleFor(o => o.FirstName).MinimumLength(2);
            RuleFor(o => o.FirstName).MaximumLength(50);

            RuleFor(o => o.LastName).NotEmpty();
            RuleFor(o => o.LastName).NotNull();
            RuleFor(o => o.LastName).MinimumLength(2);
            RuleFor(o => o.LastName).MaximumLength(50);

            RuleFor(o => o.Phone).NotEmpty();
            RuleFor(o => o.Phone).NotNull();
            RuleFor(o => o.Phone).MinimumLength(10);
            RuleFor(o => o.Phone).MaximumLength(15);
        }
    }
}
