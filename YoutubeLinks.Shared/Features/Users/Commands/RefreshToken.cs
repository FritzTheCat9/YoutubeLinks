using FluentValidation;
using MediatR;
using YoutubeLinks.Shared.Features.Users.Responses;

namespace YoutubeLinks.Shared.Features.Users.Commands
{
    public class RefreshToken
    {
        public class Command : IRequest<JwtDto>
        {
            public string RefreshToken { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {

        }
    }
}
