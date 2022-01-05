using FluentValidation;
using StudentManagementSystem.Entities.Concrete;

namespace StudentManagementSystem.Business.ValidationRules.FluentValidation
{
    public class DepartmentValidator : AbstractValidator<Department>
    {
        public DepartmentValidator()
        {
            RuleFor(d => d.DepartmentNo).NotEmpty().WithName("Bölüm kodu");
            RuleFor(d => d.DepartmentNo).NotNull().WithName("Bölüm kodu");
            RuleFor(d => d.DepartmentNo).GreaterThan(0).WithName("Bölüm kodu");

            RuleFor(d => d.DepartmentName).NotEmpty().WithName("Bölüm adı");
            RuleFor(d => d.DepartmentName).NotNull().WithName("Bölüm adı");
            RuleFor(d => d.DepartmentName).MinimumLength(5).WithName("Bölüm adı");
            RuleFor(d => d.DepartmentName).MaximumLength(200).WithName("Bölüm adı");

            RuleFor(d => d.NumberOfSemester).NotEmpty().WithName("Dönem sayısı");
            RuleFor(d => d.NumberOfSemester).NotNull().WithName("Dönem sayısı");
            RuleFor(d => d.NumberOfSemester).GreaterThan(0).WithName("Dönem sayısı");
        }
    }
}
