using System;

namespace StudentManagementSystem.Core.Utilities.Others
{
    public static class NumberStringValidator
    {
        public static bool ValidateString(string input)
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
