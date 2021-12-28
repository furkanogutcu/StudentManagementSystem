using System.Collections.Generic;
using FluentValidation;
using FluentValidation.Results;
using StudentManagementSystem.Core.Utilities.Results;

namespace StudentManagementSystem.Core.CrossCuttingConcerns.Validation.FluentValidation
{
    public static class ValidationTool
    {
        public static IDataResult<List<ValidationFailure>> Validate(IValidator validator, object entity)
        {
            var context = new ValidationContext<object>(entity);
            var result = validator.Validate(context);
            if (!result.IsValid)
            {
                return new ErrorDataResult<List<ValidationFailure>>(result.Errors,"Dogrulama basarisiz");
            }

            return new SuccessDataResult<List<ValidationFailure>>("Dogrulama basarili");
        }
    }
}