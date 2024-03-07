using FluentValidation;
using MediatR;

namespace YoutubeLinks.Shared.Features.Links.Commands
{
    public class DeleteLink
    {
        public class Command : IRequest<Unit>
        {
            public int Id { get; set; }
            public int UserId { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {

        }
    }
}
