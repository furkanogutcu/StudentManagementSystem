using FluentValidation;
using StudentManagementSystem.Entities.Concrete;

namespace StudentManagementSystem.Business.ValidationRules.FluentValidation
{
    public class InstructorValidator : AbstractValidator<Instructor>
    {
        public InstructorValidator()
        {
            RuleFor(i => i.InstructorNo).NotEmpty();
            RuleFor(i => i.InstructorNo).NotNull();
            RuleFor(i => i.InstructorNo).GreaterThan(0);

            RuleFor(i => i.DepartmentNo).NotEmpty();
            RuleFor(i => i.DepartmentNo).NotNull();
            RuleFor(i => i.DepartmentNo).GreaterThan(0);

            RuleFor(i => i.Email).NotEmpty();
            RuleFor(i => i.Email).NotNull();
            RuleFor(i => i.Email).EmailAddress();

            RuleFor(i => i.Password).NotEmpty();
            RuleFor(i => i.Password).NotNull();
            RuleFor(i => i.Password).MinimumLength(8);
            RuleFor(i => i.Password).MaximumLength(50);

            RuleFor(i => i.FirstName).NotEmpty();
            RuleFor(i => i.FirstName).NotNull();
            RuleFor(i => i.FirstName).MinimumLength(2);
            RuleFor(i => i.FirstName).MaximumLength(50);

            RuleFor(i => i.LastName).NotEmpty();
            RuleFor(i => i.LastName).NotNull();
            RuleFor(i => i.LastName).MinimumLength(2);
            RuleFor(i => i.LastName).MaximumLength(50);

            RuleFor(i => i.Phone).NotEmpty();
            RuleFor(i => i.Phone).NotNull();
            RuleFor(i => i.Phone).MinimumLength(10);
            RuleFor(i => i.Phone).MaximumLength(15);
        }
    }
}
