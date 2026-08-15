using YoutubeLinks.Api.Data.Events;
using YoutubeLinks.Api.Data.ValueObjects;
using YoutubeLinks.Shared.Exceptions;

namespace YoutubeLinks.Api.Data.Entities;

public class Playlist : Entity, IAggregateRoot
{
    public int UserId { get; private set; }
    public User User { get; private set; }

    private readonly List<Link> _links = [];
    public IReadOnlyCollection<Link> Links => _links.AsReadOnly();

    public PlaylistName Name { get; private set; }
    public bool IsPublic { get; private set; }

    private Playlist() { }

    public static Playlist Create(string name, bool isPublic, User user)
    {
        var playlist = new Playlist
        {
            Name = new PlaylistName(name),
            IsPublic = isPublic,
            User = user
        };

        playlist.AddEvent(new PlaylistCreatedDomainEvent(playlist.Id, user.Id));

        return playlist;
    }

    public Link AddLink(YoutubeUrl url, string title)
    {
        if (LinkUrlExists(url.Url))
            throw new Exception("Link already exists");
        var link = Link.Create(url, title, this);
        _links.Add(link);

        AddEvent(new LinkAddedDomainEvent(Id, link.Id, url.VideoId));
        UpdateModified();

        return link;
    }

    public Link AddLink(string url, string title)
    {
        var videoId = YoutubeUrl.ExtractVideoId(url);
        if (string.IsNullOrWhiteSpace(videoId))
            throw new Exception("Invalid youtube url");

        if (LinkUrlExists(url))
            throw new Exception("Link already exists");

        var youtubeUrl = new YoutubeUrl(url);
        var link = Link.Create(youtubeUrl, title, this);
        _links.Add(link);

        AddEvent(new LinkAddedDomainEvent(Id, link.Id, youtubeUrl.VideoId));
        UpdateModified();

        return link;
    }

    public bool LinkUrlExists(string url)
    {
        return _links.Any(x => x.Url.Url == url);
    }

    public bool LinkUrlExistsInOtherLinksThan(string url, int linkId)
    {
        return _links
            .Where(x => x.Id != linkId)
            .Any(x => x.Url.Url == url);
    }

    public void RemoveLink(int linkId)
    {
        _links.RemoveAll(x => x.Id == linkId);
        UpdateModified();
    }

    public void SetName(string name)
    {
        Name = new PlaylistName(name);
        UpdateModified();
    }

    public void SetPublic(bool isPublic)
    {
        IsPublic = isPublic;
        UpdateModified();
    }

    public void SetLinkDownloadedFlag(int linkId, bool downloaded)
    {
        var link = _links.FirstOrDefault(x => x.Id == linkId) ?? throw new MyNotFoundException();
        link.SetDownloaded(downloaded);
        UpdateModified();
    }
    public void SetLinksDownloadedFlag(bool downloaded)
    {
        _links.ForEach(x => x.SetDownloaded(downloaded));
        UpdateModified();
    }

    public Link GetLink(int linkId)
    {
        var link = _links.FirstOrDefault(x => x.Id == linkId) ?? throw new MyNotFoundException();
        return link;
    }
}