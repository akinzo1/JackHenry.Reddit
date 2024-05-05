using EventBus.Events;

namespace Reddit.API.Application.IntegrationEvents.Events;
public record UpdateRedditRequestIntegrationEvent(Guid requestId, string reddit, string statistic) : IntegrationEvent;
