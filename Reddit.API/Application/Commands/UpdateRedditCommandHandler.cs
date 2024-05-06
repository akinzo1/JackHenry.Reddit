﻿using EventBus.Abstractions;
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

            // The task is to store multiple reddit and track MostUser and Upvotes classifications.
            // Use the reddit headers limits to determine how to save the requests
            // Remember, "Your application should use these values to control throughput in an even and consistent manner while utilizing a high percentage of the available request rate."
            // Do this looping each reddit and for each classification, and use the headers to continue looping through until we run out
            // Make the service bus requests for each reddit (and under each reddit, track the classifications). Make the reddit and classifications manageable by using a list so we can increase as needed and the console will tell us what was requested.
            // We will not make any service bus requests to ping reddit api until we have enough space
            // This is because a client will be loading every x seconds strictly from cache and show the last saved dates



            // Publish to ServiceBus


            // Do a pre-validation Pipeline behavior that checks cache for current reddit limits
            //If null or enough time, then continue to regular handler which will then publish to bus

            // Load from RedditAPI

            // Store it in cache

            //var savedItem = await _redditRepository.UpdateListAsync(list);

            foreach (var statistic in request.statistics)
            {

                var updateRequestStarted = new UpdateRedditRequestIntegrationEvent(request.id, request.list, statistic);
                
                await _eventBus.PublishAsync(updateRequestStarted);

                _logger.LogInformation("Publishing integration event: {IntegrationEventId_published} - ({@IntegrationEvent})", updateRequestStarted.Id, updateRequestStarted);

                // return count of rateLimits to use to determine when next to load

                // check cache for rate limits 
                // check cache to see if there are other stats needing to be updated that are getting stale


                // if IsOk
                //      make call to service bus for this reddit and statistic
                //          event bus handler check cache again and then will make request to reddit api 
                //          update new limits into cache
                //          update cache with api results

                // otherwise




            }

            var cached = await _redditRepository.GetLimitsAsync() ?? new ApiLimits() { RateLimit_Remaining = 0, RateLimit_Reset = 0, RateLimit_Used = 0 };

            return await Task.FromResult(cached);

        }
    }
}
