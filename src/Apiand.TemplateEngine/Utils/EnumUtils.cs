using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Apiand.TemplateEngine.Utils;

public static class EnumUtils
{
    /// <summary>
    /// Converts an enum value to a human-readable string (e.g., MultiLayer becomes "multi-layer")
    /// </summary>
    /// <param name="value">The enum value to humanize</param>
    /// <returns>A human-readable string representation</returns>
    public static string Humanize(this Enum value)
    {
        string name = value.ToString();

        // Add a space before each capital letter and make it lowercase
        string result = Regex.Replace(name, "([A-Z])", " $1").Trim();

        // Replace spaces with hyphens
        return Regex.Replace(result, @"\s+", "-");
    }

    /// <summary>
    /// Attempts to convert a humanized string back to an enum value.
    /// </summary>
    /// <typeparam name="T">The enum type</typeparam>
    /// <param name="humanizedString">The humanized string (e.g., "multi-layer")</param>
    /// <returns>The corresponding enum value, or null if conversion fails</returns>
    public static T? Dehumanize<T>(this string humanizedString) where T : struct, Enum
    {
        if (string.IsNullOrEmpty(humanizedString))
            return null;

        // Replace hyphens with spaces
        string spacedString = humanizedString.Replace("-", " ");

        // Convert to title case (first letter of each word capitalized)
        string pascalCase = System.Globalization.CultureInfo.CurrentCulture.TextInfo
            .ToTitleCase(spacedString)
            .Replace(" ", ""); // Remove spaces

        // Try to parse the string to the enum type
        if (Enum.TryParse<T>(pascalCase, out T result))
            return result;

        return null;
    }

    /// <summary>
    /// Gets all values from an enum type and humanizes them
    /// </summary>
    /// <typeparam name="T">The enum type</typeparam>
    /// <returns>An array of humanized enum values</returns>
    public static T[] GetAll<T>() where T : Enum
    {
        return Enum.GetValues(typeof(T)).Cast<T>().ToArray();
    }
}