using System.Linq;
using System.Text.RegularExpressions;

public class StringParser 
{
    // Helper method to convert from CamelCase to snake_case
    public static string ToSnakeCase(string input)
    {
        return Regex.Replace(input, "(?<=.)([A-Z])", "_$1", RegexOptions.Compiled).TrimStart('_').ToUpper();
    }

    // Helper method to convert from snake_case to CamelCase
    public static string ToCamelCase(string input)
    {
        string[] parts = input.Split('_');
        return string.Join("", parts.Select(p => char.ToUpper(p[0]) + p[1..].ToLower()));
    }
}
