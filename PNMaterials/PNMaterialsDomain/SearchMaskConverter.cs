using System.Text;

namespace PNMaterialsDomain;

public static class SearchMaskConverter
{
    public const string EscapeCharacter = "\\";

    // Превращает пользовательскую маску («Кабель*») в SQL-шаблон LIKE («Кабель%»).
    public static string ToLikePattern(string? mask)
    {
        if (string.IsNullOrWhiteSpace(mask))
            return "%";

        var sb = new StringBuilder();

        foreach (var ch in mask.Trim())
        {
            switch (ch)
            {
                case '*':
                    sb.Append('%');
                    break;

                case '%':
                case '_':
                case '[':
                case '\\':
                    sb.Append(EscapeCharacter).Append(ch);
                    break;

                default:
                    sb.Append(ch);
                    break;
            }
        }

        return sb.ToString();
    }
}