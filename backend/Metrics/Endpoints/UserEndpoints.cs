using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Shared;

namespace Metrics;

public static class MetricsEndpoints
{
    public static IEndpointRouteBuilder AddMetricsEndpoints(this IEndpointRouteBuilder builder)
    {
        builder
            .MapGroup(MetricsContexts.Endpoint)
            .MapPost(MetricsContexts.Level.Name, Level);

        return builder;
    }

    private static async Task Level(
        [FromBody] MetricsContexts.Level request,
        [FromServices] IMetricsPublisher publisher)
    {
        await publisher.Publish(new LevelMetrics.Migration(), request.ToPayload());
    }
}