namespace YoutubeLinks.Api.Abstractions;

public interface IClock
{
    DateTime Current();
}

public class Clock : IClock
{
    public DateTime Current() 
        => DateTime.UtcNow;
}