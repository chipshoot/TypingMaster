using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace TypingMaster.Business;

public static class Extensions
{
    public static string Repeat(this string value, int count)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        var text = count < 0 ? value : new StringBuilder().Insert(0, value + " ", count).ToString().TrimEnd();
        return text;
    }

    public static bool IsDateTimeTemplate(this string pattern)
    {
        var dateTimeSpecifiers = new[] { "y", "M", "d", "H", "h", "m", "s", "t" };

        var result = dateTimeSpecifiers.Any(pattern.Contains);
        return result;
    }
}