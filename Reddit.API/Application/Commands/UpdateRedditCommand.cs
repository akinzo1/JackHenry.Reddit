using MediatR;

namespace Reddit.API;

public record UpdateRedditCommand(string list) : IRequest<string>;
