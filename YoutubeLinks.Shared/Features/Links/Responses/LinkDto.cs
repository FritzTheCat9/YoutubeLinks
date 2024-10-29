﻿namespace YoutubeLinks.Shared.Features.Links.Responses;

public class LinkDto
{
    public int Id { get; init; }
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }

    public string Url { get; set; }
    public string VideoId { get; set; }
    public string Title { get; set; }
    public bool Downloaded { get; set; }
    public int PlaylistId { get; set; }
}