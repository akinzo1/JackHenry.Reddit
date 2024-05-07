using EventBus.Abstractions;
using MediatR;
using Reddit.API.Application.IntegrationEvents.Events;
using Reddit.API.Model;
using Reddit.API.Model.Api;

namespace Reddit.API.Application.Commands
{
    public class UpdateRedditCommandHandler : IRequestHandler<UpdateRedditCommand, ApiLimits>
    {
        private readonly IRedditRepository _redditRepository;
        private readonly ILogger<UpdateRedditCommandHandler> _logger;
        private readonly IEventBus _eventBus;

        public UpdateRedditCommandHandler(IEventBus eventBus, IRedditRepository redditRepository,
        ILogger<UpdateRedditCommandHandler> logger)

        {
            _redditRepository = redditRepository;
            _logger = logger;
            _eventBus = eventBus;

        }

        public async Task<ApiLimits> Handle(UpdateRedditCommand request, CancellationToken cancellationToken)
        {

            try
            {
                foreach (var statistic in request.statistics)
                {

                    var updateRequestStarted = new UpdateRedditRequestIntegrationEvent(request.id, request.list, statistic);

                    await _eventBus.PublishAsync(updateRequestStarted);

                    _logger.LogInformation("Publishing integration event: {IntegrationEventId_published} - ({@IntegrationEvent})", updateRequestStarted.Id, updateRequestStarted);

                }

                var cached = await _redditRepository.GetLimitsAsync() ?? new ApiLimits() { RateLimit_Remaining = 0, RateLimit_Reset = 0, RateLimit_Used = 0 };
                return await Task.FromResult(cached);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed: {ex}");
                throw;
            }


        }
    }
}
