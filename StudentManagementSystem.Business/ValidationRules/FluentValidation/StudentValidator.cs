using System;
using FluentValidation;
using StudentManagementSystem.Business.Constants;
using StudentManagementSystem.Entities.Concrete;

namespace StudentManagementSystem.Business.ValidationRules.FluentValidation
{
    public class StudentValidator : AbstractValidator<Student>
    {
        public StudentValidator()
        {
            RuleFor(s => s.StudentNo).NotEmpty().WithName("Öğrenci no");
            RuleFor(s => s.StudentNo).NotNull().WithName("Öğrenci no");
            RuleFor(s => s.StudentNo).GreaterThan(0).WithName("Öğrenci no");

            RuleFor(s => s.DepartmentNo).NotEmpty().WithName("Bölüm kodu");
            RuleFor(s => s.DepartmentNo).NotNull().WithName("Bölüm kodu");
            RuleFor(s => s.DepartmentNo).GreaterThan(0).WithName("Bölüm kodu");

            RuleFor(s => s.AdviserNo).NotEmpty().WithName("Danışman no");
            RuleFor(s => s.AdviserNo).NotNull().WithName("Danışman no");
            RuleFor(s => s.AdviserNo).GreaterThan(0).WithName("Danışman no");

            RuleFor(s => s.Email).NotEmpty();
            RuleFor(s => s.Email).NotNull();
            RuleFor(s => s.Email).EmailAddress();

            RuleFor(s => s.Password).NotEmpty().WithName("Şifre");
            RuleFor(s => s.Password).NotNull().WithName("Şifre");
            RuleFor(s => s.Password).MinimumLength(6).WithName("Şifre");
            RuleFor(s => s.Password).MaximumLength(50).WithName("Şifre");

            RuleFor(s => s.FirstName).NotEmpty().WithName("Ad"); ;
            RuleFor(s => s.FirstName).NotNull().WithName("Ad");
            RuleFor(s => s.FirstName).MinimumLength(3).WithName("Ad");
            RuleFor(s => s.FirstName).MaximumLength(50).WithName("Ad");

            RuleFor(s => s.LastName).NotEmpty().WithName("Soyad");
            RuleFor(s => s.LastName).NotNull().WithName("Soyad");
            RuleFor(s => s.LastName).MinimumLength(3).WithName("Soyad");
            RuleFor(s => s.LastName).MaximumLength(50).WithName("Soyad");

            RuleFor(s => s.Phone).NotEmpty().WithName("Telefon");
            RuleFor(s => s.Phone).NotNull().WithName("Telefon");
            RuleFor(s => s.Phone).MinimumLength(10).WithName("Telefon");
            RuleFor(s => s.Phone).MaximumLength(15).WithName("Telefon");

            RuleFor(s => s.Semester).NotEmpty().WithName("Dönem");
            RuleFor(s => s.Semester).NotNull().WithName("Dönem");
            RuleFor(s => s.Semester).GreaterThan(0).WithName("Dönem");
            RuleFor(s => s.Phone).Must(NumberString).WithMessage(Messages.OnlyNumberForPhone).WithName("Dönem");

            RuleFor(s => s.EnrollmentDate).NotEmpty().WithName("Kayıt tarihi");
            RuleFor(s => s.EnrollmentDate).NotNull().WithName("Kayıt tarihi");
            RuleFor(s => s.EnrollmentDate).LessThanOrEqualTo(short.Parse(DateTime.Now.Year.ToString())).WithName("Kayıt tarihi");
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
