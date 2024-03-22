namespace YoutubeLinks.Shared.Features.Links.Helpers
{
    public class YoutubeFile
    {
        public byte[] FileBytes { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
        public YoutubeFileType YoutubeFileType { get; set; }
    }
}
