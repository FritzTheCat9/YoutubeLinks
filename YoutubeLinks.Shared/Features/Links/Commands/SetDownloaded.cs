using FluentValidation;
using MediatR;

namespace YoutubeLinks.Shared.Features.Links.Commands
{
    public class SetDownloaded
    {
        public class Command : IRequest<Unit>
        {
            public int Id { get; set; }
            public bool Downloaded { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {

        }
    }
}
