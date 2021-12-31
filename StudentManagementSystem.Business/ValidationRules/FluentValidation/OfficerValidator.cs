using System;
using FluentValidation;
using StudentManagementSystem.Business.Constants;
using StudentManagementSystem.Entities.Concrete;

namespace StudentManagementSystem.Business.ValidationRules.FluentValidation
{
    public class OfficerValidator : AbstractValidator<Officer>
    {
        public OfficerValidator()
        {
            RuleFor(o => o.OfficerNo).NotEmpty().WithName("Memur no");
            RuleFor(o => o.OfficerNo).NotNull().WithName("Memur no");
            RuleFor(o => o.OfficerNo).GreaterThan(0).WithName("Memur no");

            RuleFor(o => o.Email).NotEmpty();
            RuleFor(o => o.Email).NotNull();
            RuleFor(o => o.Email).EmailAddress();

            RuleFor(o => o.Password).NotEmpty().WithName("Şifre");
            RuleFor(o => o.Password).NotNull().WithName("Şifre");
            RuleFor(o => o.Password).MinimumLength(8).WithName("Şifre");
            RuleFor(o => o.Password).MaximumLength(50).WithName("Şifre");

            RuleFor(o => o.FirstName).NotEmpty().WithName("Ad");
            RuleFor(o => o.FirstName).NotNull().WithName("Ad");
            RuleFor(o => o.FirstName).MinimumLength(2).WithName("Ad");
            RuleFor(o => o.FirstName).MaximumLength(50).WithName("Ad");

            RuleFor(o => o.LastName).NotEmpty().WithName("Soyad");
            RuleFor(o => o.LastName).NotNull().WithName("Soyad");
            RuleFor(o => o.LastName).MinimumLength(2).WithName("Soyad");
            RuleFor(o => o.LastName).MaximumLength(50).WithName("Soyad");

            RuleFor(o => o.Phone).NotEmpty().WithName("Telefon");
            RuleFor(o => o.Phone).NotNull().WithName("Telefon");
            RuleFor(o => o.Phone).MinimumLength(10).WithName("Telefon");
            RuleFor(o => o.Phone).MaximumLength(15).WithName("Telefon");
            RuleFor(o => o.Phone).Must(NumberString).WithMessage(Messages.OnlyNumberForPhone).WithName("Telefon"); ;
            
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
