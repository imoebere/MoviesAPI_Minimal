namespace MoviesAPI_Minimal.Validation
{
    public static class ValidationUtilities
    {
        public static string NonEmptyMessage = "The field {PropertyName} is required";
        public static string MaximumLengthMessage = "The field {PropertyName} should be less than {MaxLength} characters";
        public static string FirstLettterIsUpperCaseMessage = "The field {PropertyName} should start with uppercase";
        public static string GreaterThanDate(DateTime value) => "The field {PropertyName} should be greater than " + value.ToString("yyyy-MM-dd");

        public static bool FirstLetterIsUpperCase(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            var firstLetter = value[0].ToString();
            return firstLetter == firstLetter.ToUpper();
        }
    }
}
