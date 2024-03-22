namespace YoutubeLinks.Shared.Features.Playlists.Helpers
{
    public static class PlaylistHelpers
    {
        public static string PlaylistFileTypeToString(PlaylistFileType playlistFileType)
        {
            return playlistFileType switch
            {
                PlaylistFileType.TXT => "txt",
                _ => "json",
            };
        }
    }
}
