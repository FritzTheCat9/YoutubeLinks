using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace YoutubeLinks.Shared.Features.Links.Helpers
{
    public static class YoutubeHelpers
    {
        [StringSyntax(StringSyntaxAttribute.Regex)]
        public static readonly string VideoIdRegex = @"(?:\?|&)v=([^&]+)";

        public static readonly string VideoPathBase = "https://www.youtube.com/watch?v=";

        public static string GetVideoId(string videoUrl)
        {
            var regex = new Regex(VideoIdRegex);
            var match = regex.Match(videoUrl);

            return match.Success ? match.Groups[1].Value : null;
        }

        public static string YoutubeFileTypeToString(YoutubeFileType youtubeFileType)
        {
            return youtubeFileType switch
            {
                YoutubeFileType.MP4 => "mp4",
                _ => "mp3",
            };
        }
    }
}
