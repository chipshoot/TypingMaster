using System.Text.RegularExpressions;
namespace TypingMaster.Tests.Utility;

public static class StringChecker
{
    public static bool ContainsOnlyAllowedChars(this string input, char[] allowedChars)
    {
        if (string.IsNullOrEmpty(input))
        {
            return true;
        }

        var allowedCharsWithSpace = GetAllowedCharsWithSpace(allowedChars);
        return input.All(c => allowedCharsWithSpace.Contains(c));
    }

    public static bool ContainsOnlyAllowedCharsIgnoreCase(string input, char[] allowedChars)
    {
        if (string.IsNullOrEmpty(input))
        {
            return true;
        }


        var lowerAllowedChars = allowedChars.Select(char.ToLower).ToArray();
        var allowedCharsWithSpace = GetAllowedCharsWithSpace(lowerAllowedChars);
        return input.ToLower().All(c => allowedCharsWithSpace.Contains(c));
    }

    public static bool ContainsOnlyAllowedCharsWithHashSet(string input, char[] allowedChars)
    {
        if (string.IsNullOrEmpty(input))
        {
            return true;
        }

        var allowedCharsWithSpace = GetAllowedCharsWithSpace(allowedChars);
        var allowedCharSet = new HashSet<char>(allowedCharsWithSpace);
        return input.All(allowedCharSet.Contains);
    }

    public static bool ContainsOnlyAllowedCharsIgnoreCaseWithHashSet(string input, char[] allowedChars)
    {
        if (string.IsNullOrEmpty(input))
        {
            return true;
        }

        var allowedCharsWithSpace = GetAllowedCharsWithSpace(allowedChars);
        var allowedCharSet = new HashSet<char>(allowedCharsWithSpace.Select(char.ToLower));
        return input.ToLower().All(allowedCharSet.Contains);
    }

    public static bool ContainsOnlyAllowedCharsWithRegex(string input, char[] allowedChars)
    {
        if (string.IsNullOrEmpty(input))
        {
            return true;
        }

        var allowedCharsWithSpace = GetAllowedCharsWithSpace(allowedChars);
        string pattern = "^[" + string.Join("", allowedCharsWithSpace) + "]*$";
        return Regex.IsMatch(input, pattern);
    }

    public static bool ContainsOnlyAllowedCharsIgnoreCaseWithRegex(string input, char[] allowedChars)
    {
        if (string.IsNullOrEmpty(input))
        {
            return true;
        }

        var allowedCharsWithSpace = GetAllowedCharsWithSpace(allowedChars);
        string pattern = "^[" + string.Join("", allowedCharsWithSpace).ToLower() + string.Join("", allowedCharsWithSpace).ToUpper() +
                         "]*$";
        return Regex.IsMatch(input, pattern);
    }

    private static char[] GetAllowedCharsWithSpace(char[] allowedChars)
    {
        var allowedCharsWithSpace = new char[allowedChars.Length + 1];
        Array.Copy(allowedChars, allowedCharsWithSpace, allowedChars.Length);
        allowedCharsWithSpace[allowedChars.Length] = ' ';
        return allowedCharsWithSpace;
    }
}