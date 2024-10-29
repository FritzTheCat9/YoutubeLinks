﻿using FluentValidation;
using MediatR;

namespace YoutubeLinks.Shared.Features.Links.Commands;

public static class SetLinkDownloadedFlag
{
    public class Command : IRequest<Unit>
    {
        public int Id { get; set; }
        public bool Downloaded { get; init; }
    }

    public class Validator : AbstractValidator<Command> { }
}