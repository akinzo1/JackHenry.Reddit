using EventBus.Events;

namespace Reddit.API.Application.IntegrationEvents.Events;
public record UpdateRedditRequestIntegrationEvent(Guid requestId, string subReddit, string statistic) : IntegrationEvent;
