﻿using FluentValidation;
using MediatR;
using Microsoft.Extensions.Localization;
using YoutubeLinks.Shared.Localization;

namespace YoutubeLinks.Shared.Features.Playlists.Commands;

public static class UpdatePlaylist
{
    public class Command : IRequest<Unit>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Public { get; set; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator(IStringLocalizer<ValidationMessage> localizer)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(x => localizer[nameof(ValidationMessageString.NameNotEmpty)])
                .MaximumLength(ValidationConsts.MaximumStringLength)
                .WithMessage(x => localizer[nameof(ValidationMessageString.NameMaximumLength)]);
        }
    }
}