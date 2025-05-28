using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Shared;

namespace Metrics;

public static class MetricsEndpoints
{
    public static IEndpointRouteBuilder AddMetricsEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup(MetricsContexts.EndpointGroup);
        
        group.MapPost(MetricsContexts.LevelPass.Name, LevelPass);
        group.MapPost(MetricsContexts.LevelRate.Name, LevelRate);

        return builder;
    }

    private static async Task LevelPass(
        [FromBody] MetricsContexts.LevelPass request,
        [FromServices] IMetricsPublisher publisher)
    {
        await publisher.Publish(request.ToPayload());
    }
    
    private static async Task LevelRate(
        [FromBody] MetricsContexts.LevelRate request,
        [FromServices] IMetricsPublisher publisher)
    {
        await publisher.Publish(request.ToPayload());
    }
}