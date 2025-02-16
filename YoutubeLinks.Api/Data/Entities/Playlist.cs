using YoutubeLinks.Shared.Exceptions;

namespace YoutubeLinks.Api.Data.Entities;

public class Playlist : Entity, IAggregateRoot
{
    public int UserId { get; private set; }
    public User User { get; private set; }

    private readonly List<Link> _links = [];
    public IReadOnlyCollection<Link> Links => _links.AsReadOnly();

    public string Name { get; private set; }
    public bool Public { get; private set; }

    private Playlist() { }

    public static Playlist Create(string name, bool isPublic, User user)
    {
        return new Playlist
        {
            Name = name,
            Public = isPublic,
            User = user
        };
    }

    public Link AddLink(string url, string videoId, string title)
    {
        // validate data
        // check LinkUrlExists

        var link = Link.Create(url, videoId, title, this);
        _links.Add(link);
        UpdateModified();
        return link;
    }

    public bool LinkUrlExists(string url)
    {
        return _links.Any(x => x.Url == url);
    }

    public bool LinkUrlExistsInOtherLinksThan(string url, int linkId)
    {
        return _links
            .Where(x => x.Id != linkId)
            .Any(x => x.Url == url);
    }

    public void RemoveLink(int linkId)
    {
        _links.RemoveAll(x => x.Id == linkId);
        UpdateModified();
    }

    public void SetName(string name)
    {
        Name = name;
        UpdateModified();
    }

    public void SetPublic(bool isPublic)
    {
        Public = isPublic;
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