using MediatR;
using Reddit.API.Model.Api;

namespace Reddit.API;

public record UpdateRedditCommand(Guid id, string list, string[] statistics) : IRequest<ApiLimits>;
