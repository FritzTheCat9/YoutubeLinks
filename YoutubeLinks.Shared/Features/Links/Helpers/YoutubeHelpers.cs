using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace YoutubeLinks.Shared.Features.Links.Helpers
{
    public static class YoutubeHelpers
    {
        [StringSyntax(StringSyntaxAttribute.Regex)]
        public const string VideoIdRegex = @"(?:\?|&)v=([^&]+)";

        public const string VideoPathBase = "https://www.youtube.com/watch?v=";

        public const int MaximumTitleLength = 255;

        public static string GetVideoId(string videoUrl)
        {
            var regex = new Regex(VideoIdRegex);
            var match = regex.Match(videoUrl);

            return match.Success ? match.Groups[1].Value.Trim() : null;
        }

        public static string YoutubeFileTypeToString(YoutubeFileType youtubeFileType)
        {
            return youtubeFileType switch
            {
                YoutubeFileType.Mp4 => "mp4",
                _ => "mp3",
            };
        }

        public static string NormalizeVideoTitle(string title)
        {
            var invalidChars = GetInvalidPathAndFileNameCharacters();

            var sanitizedTitle = Regex.Replace(title, "[" + Regex.Escape(invalidChars) + "]", "");
            sanitizedTitle = Regex.Replace(sanitizedTitle, @"\s+", " ");
            sanitizedTitle = sanitizedTitle.Trim();

            if (sanitizedTitle.Length > MaximumTitleLength)
                sanitizedTitle = sanitizedTitle.Substring(0, MaximumTitleLength);

            return sanitizedTitle;
        }

        public static bool HaveValidCharactersInTitle(string title)
        {
            var invalidChars = GetInvalidPathAndFileNameCharacters();
            return !ContainsInvalidChars(title, invalidChars);
        }

        private static string GetInvalidPathAndFileNameCharacters() 
            => new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());

        private static bool ContainsInvalidChars(string input, string invalidChars) 
            => input.IndexOfAny(invalidChars.ToCharArray()) != -1;
    }
}
