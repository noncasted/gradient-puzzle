using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Shared;

namespace Users;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder AddUserEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapPost("getProgress", GetProgress);
        builder.MapPost("setLevelPassed", SetLevelPassed);

        return builder;
    }

    private static async Task<GetUserProgress.Response> GetProgress(
        [FromBody] GetUserProgress.Request request,
        [FromServices] IGrainFactory grains)
    {
        var userProgress = grains.GetGrain<IUserProgress>(request.UserId);
        var state = await userProgress.GetProgress();

        var response = new GetUserProgress.Response
        {
            PassedLevels = state.PassedLevels.ToList()
        };

        return response;
    }

    private static Task SetLevelPassed(
        [FromBody] SetUserProgress.Request request,
        [FromServices] IGrainFactory grains)
    {
        var userProgress = grains.GetGrain<IUserProgress>(request.UserId);
        return userProgress.OnLevelPassed(request.LevelId);
    }
}