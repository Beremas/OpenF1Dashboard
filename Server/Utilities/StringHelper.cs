namespace OpenF1Dashboard.Shared.Utilities
{
    public class StringHelper
    {
        public static string Capitalize(string text) => string.IsNullOrEmpty(text) ? text : char.ToUpper(text[0]) + text.Substring(1).ToLower();

    }
}
