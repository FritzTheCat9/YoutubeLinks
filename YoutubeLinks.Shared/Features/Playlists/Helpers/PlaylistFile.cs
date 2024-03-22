namespace YoutubeLinks.Shared.Features.Playlists.Helpers
{
    public class PlaylistFile
    {
        public byte[] FileBytes { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
        public PlaylistFileType PlaylistFileType { get; set; }
    }

    public class PlaylistModel
    {
        public int LinksCount { get; set; }
        public IEnumerable<LinkModel> LinkModels { get; set; }
    }

    public class LinkModel
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string VideoId { get; set; }
    }
}
