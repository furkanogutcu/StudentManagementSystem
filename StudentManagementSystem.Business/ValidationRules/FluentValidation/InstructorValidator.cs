using System;
using FluentValidation;
using StudentManagementSystem.Business.Constants;
using StudentManagementSystem.Entities.Concrete;

namespace StudentManagementSystem.Business.ValidationRules.FluentValidation
{
    public class InstructorValidator : AbstractValidator<Instructor>
    {
        public InstructorValidator()
        {
            RuleFor(i => i.InstructorNo).NotEmpty().WithName("Öğretim görevlisi no");
            RuleFor(i => i.InstructorNo).NotNull().WithName("Öğretim görevlisi no");
            RuleFor(i => i.InstructorNo).GreaterThan(0).WithName("Öğretim görevlisi no");

            RuleFor(i => i.DepartmentNo).NotEmpty().WithName("Bölüm kodu");
            RuleFor(i => i.DepartmentNo).NotNull().WithName("Bölüm kodu");
            RuleFor(i => i.DepartmentNo).GreaterThan(0).WithName("Bölüm kodu");

            RuleFor(i => i.Email).NotEmpty();
            RuleFor(i => i.Email).NotNull();
            RuleFor(i => i.Email).EmailAddress();

            RuleFor(i => i.Password).NotEmpty().WithName("Şifre");
            RuleFor(i => i.Password).NotNull().WithName("Şifre");
            RuleFor(i => i.Password).MinimumLength(6).WithName("Şifre");
            RuleFor(i => i.Password).MaximumLength(50).WithName("Şifre");

            RuleFor(i => i.FirstName).NotEmpty().WithName("Ad");
            RuleFor(i => i.FirstName).NotNull().WithName("Ad");
            RuleFor(i => i.FirstName).MinimumLength(3).WithName("Ad");
            RuleFor(i => i.FirstName).MaximumLength(50).WithName("Ad");

            RuleFor(i => i.LastName).NotEmpty().WithName("Soyad");
            RuleFor(i => i.LastName).NotNull().WithName("Soyad");
            RuleFor(i => i.LastName).MinimumLength(3).WithName("Soyad");
            RuleFor(i => i.LastName).MaximumLength(50).WithName("Soyad");

            RuleFor(i => i.Phone).NotEmpty().WithName("Telefon");
            RuleFor(i => i.Phone).NotNull().WithName("Telefon");
            RuleFor(i => i.Phone).MinimumLength(10).WithName("Telefon");
            RuleFor(i => i.Phone).MaximumLength(15).WithName("Telefon");
            RuleFor(i => i.Phone).Must(NumberString).WithMessage(Messages.OnlyNumberForPhone).WithName("Telefon");
        }
        private bool NumberString(string input)
        {
            foreach (var chr in input)
            {
                if (!Char.IsNumber(chr))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
