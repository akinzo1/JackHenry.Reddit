﻿using MediatR;

namespace Reddit.API;

public record UpdateRedditCommand(Guid id, string list, string[] statistics) : IRequest<(string RedditListName, int Remaining, int Used, int Reset)>;
