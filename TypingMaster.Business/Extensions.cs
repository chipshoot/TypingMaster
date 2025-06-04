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
}