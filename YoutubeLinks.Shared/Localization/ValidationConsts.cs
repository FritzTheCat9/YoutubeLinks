using System.Diagnostics.CodeAnalysis;

namespace YoutubeLinks.Shared.Localization;

public static class ValidationConsts
{
    public const int MaximumStringLength = 50;
    public const int MinimumStringLength = 7;

    [StringSyntax(StringSyntaxAttribute.Regex)]
    public const string UserNameRegex = "^[a-zA-Z0-9_]+$";

    [StringSyntax(StringSyntaxAttribute.Regex)]
    public const string YoutubeVideoUrlRegex =
        "^(?:https?:\\/\\/)?(?:www\\.)?(?:youtube\\.com\\/(?:[^\\/\\n\\s]+\\/\\S+\\/|(?:v|e(?:mbed)?)\\/|\\S*?[?&]v=)|youtu\\.be\\/)([a-zA-Z0-9_-]{11})";
}