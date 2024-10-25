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

    public class JsonPlaylistExporter : IPlaylistExporter
    {
        public PlaylistFile Export(Playlist playlist)
        {
            var playlistModel = playlist.GetPlaylistModel();

            var jsonPlaylistFile = new PlaylistFile()
            {
                FileBytes = JsonSerializer.SerializeToUtf8Bytes(playlistModel),
                FileName = $"{playlist.Name}.json",
                PlaylistFileType = PlaylistFileType.Json,
                ContentType = $"application/json",
            };

            return jsonPlaylistFile;
        }
    }

    public class TxtPlaylistExporter : IPlaylistExporter
    {
        public PlaylistFile Export(Playlist playlist)
        {
            var playlistModel = playlist.GetPlaylistModel();

            var txtPlaylistFile = new PlaylistFile()
            {
                FileBytes = GetPlaylistTxtFileBytes(playlistModel.LinkModels),
                FileName = $"{playlist.Name}.txt",
                PlaylistFileType = PlaylistFileType.Txt, 
                ContentType = $"text/plain",
            };

            return txtPlaylistFile;
        }

        private static byte[] GetPlaylistTxtFileBytes(IEnumerable<LinkJsonModel> links)
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
                PlaylistFileType.Txt => new TxtPlaylistExporter(),
                _ => new JsonPlaylistExporter(),
            };
        }
    }
}