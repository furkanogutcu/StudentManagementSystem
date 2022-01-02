using StudentManagementSystem.Core.Utilities.Others;

namespace StudentManagementSystem.Application.Utilities
{
    public static class PasswordCreator
    {
        public static string Create(string firstField, string secondField)
        {
            return $"{TurkishCharNormalizer.Normalization(firstField.ToLower())}.{TurkishCharNormalizer.Normalization(secondField.ToLower())}";
        }
    }
}
