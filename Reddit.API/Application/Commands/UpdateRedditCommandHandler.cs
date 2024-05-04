using MediatR;

namespace Reddit.API.Application.Commands
{
    public class UpdateRedditCommandHandler : IRequestHandler<UpdateRedditCommand, string>
    {
        private readonly IRedditRepository _redditRepository;

        public UpdateRedditCommandHandler(IRedditRepository redditRepository)
        {
            _redditRepository = redditRepository;
        }

        public async Task<string> Handle(UpdateRedditCommand request, CancellationToken cancellationToken)
        {
            var tt = await _redditRepository.UpdateListAsync(new Model.RedditList() { ListName = "funny"});

            
            return await Task.FromResult<string>(tt.ListName);
            //throw new NotImplementedException();
        }
    }
}
