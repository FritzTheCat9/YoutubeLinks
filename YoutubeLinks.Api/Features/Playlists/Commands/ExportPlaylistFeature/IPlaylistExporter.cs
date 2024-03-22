using System.Text;
using System.Text.Json;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Api.Features.Playlists.Extensions;
using YoutubeLinks.Shared.Features.Playlists.Helpers;

namespace YoutubeLinks.Api.Features.Playlists.Commands.ExportPlaylistFeature
{
    public interface IPlaylistExporter
    {
        PlaylistFile Export(Playlist playlist);
    }

    public class JSONPlaylistExporter : IPlaylistExporter
    {
        public PlaylistFile Export(Playlist playlist)
        {
            var playlistModel = playlist.GetPlaylistModel();

            var jsonPlaylistFile = new PlaylistFile()
            {
                FileBytes = JsonSerializer.SerializeToUtf8Bytes(playlistModel),
                FileName = $"{playlist.Name}.json",
                PlaylistFileType = PlaylistFileType.JSON,
                ContentType = $"application/json",
            };

            return jsonPlaylistFile;
        }
    }

    public class TXTPlaylistExporter : IPlaylistExporter
    {
        public PlaylistFile Export(Playlist playlist)
        {
            var playlistModel = playlist.GetPlaylistModel();

            var txtPlaylistFile = new PlaylistFile()
            {
                FileBytes = GetPlaylistTxtFileBytes(playlistModel.LinkModels),
                FileName = $"{playlist.Name}.txt",
                PlaylistFileType = PlaylistFileType.TXT, 
                ContentType = $"text/plain",
            };

            return txtPlaylistFile;
        }

        private static byte[] GetPlaylistTxtFileBytes(IEnumerable<LinkModel> links)
        {
            var stringBuilder = new StringBuilder();
            foreach (var link in links)
            {
                stringBuilder.AppendLine(link.Url);
            }
            var fileText = stringBuilder.ToString();

            var fileBytes = Encoding.UTF8.GetBytes(fileText);
            return fileBytes;
        }
    }

    public static class PlaylistExporterHelpers
    {
        public static IPlaylistExporter GetExporter(PlaylistFileType fileType)
        {
            return fileType switch
            {
                PlaylistFileType.TXT => new TXTPlaylistExporter(),
                _ => new JSONPlaylistExporter(),
            };
        }
    }
}