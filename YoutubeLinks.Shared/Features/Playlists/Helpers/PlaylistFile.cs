namespace YoutubeLinks.Shared.Features.Playlists.Helpers
{
    public class PlaylistFile
    {
        public byte[] FileBytes { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
        public PlaylistFileType PlaylistFileType { get; set; }
    }

    public class PlaylistJSONModel
    {
        public int LinksCount { get; set; }
        public IEnumerable<LinkJSONModel> LinkModels { get; set; }
    }

    public class LinkJSONModel
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string VideoId { get; set; }
    }
}
