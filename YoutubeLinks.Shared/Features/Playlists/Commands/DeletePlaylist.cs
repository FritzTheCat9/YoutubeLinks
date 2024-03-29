﻿using FluentValidation;
using MediatR;

namespace YoutubeLinks.Shared.Features.Playlists.Commands
{
    public class DeletePlaylist
    {
        public class Command : IRequest<Unit>
        {
            public int Id { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {

        }
    }
}
