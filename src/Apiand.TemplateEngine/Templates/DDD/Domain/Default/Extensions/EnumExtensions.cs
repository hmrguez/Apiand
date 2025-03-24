using System.Text.RegularExpressions;

namespace XXXnameXXX.Domain.Extensions;

public static class EnumExtensions
{
    public static string Humanize(this Enum enumValue)
    {
        var enumString = enumValue.ToString();
        var humanizedString = Regex.Replace(enumString, "(\\B[A-Z])", " $1");
        return humanizedString;
    }
}