using YoutubeLinks.Api.Data.Entities;

namespace YoutubeLinks.UnitTests.Builders;

public class PlaylistBuilder
{   
    private string _name = "MyPlaylist";
    private bool _isPublic = false;
    private User _user = new UserBuilder().Build();
    private readonly List<(string url, string id, string title)> _links = new();

    public PlaylistBuilder WithName(string name) { _name = name; return this; }
    public PlaylistBuilder Public(bool isPublic = true) { _isPublic = isPublic; return this; }
    public PlaylistBuilder WithUser(User user) { _user = user; return this; }

    public PlaylistBuilder WithLink(string url, string videoId, string title = "")
    {
        _links.Add((url, videoId, title));
        return this;
    }

    public Playlist Build()
    {
        var playlist = Playlist.Create(_name, _isPublic, _user);
        foreach (var l in _links)
            playlist.AddLink(l.url, l.title);

        return playlist;
    }

    public static PlaylistBuilder Create() => new();
}
