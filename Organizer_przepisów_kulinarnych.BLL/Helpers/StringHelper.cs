
namespace Organizer_przepisów_kulinarnych.BLL.Helpers
{
    public static class StringHelper
    {
        public static string CapitalizeFirstLetter(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return char.ToUpper(input[0]) + input.Substring(1).ToLower();
        }

        public static bool FuzzyMatch(string str1, string str2)
        {
            str1 = str1.Trim().ToLowerInvariant();
            str2 = str2.Trim().ToLowerInvariant();

            var tokenSetScore = FuzzySharp.Fuzz.Ratio(str1, str2);

            return tokenSetScore >= 75;
        }
    }
}
