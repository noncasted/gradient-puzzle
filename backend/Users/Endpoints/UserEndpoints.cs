using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Shared;

namespace Users;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder AddUserEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(UserContexts.GetProgress.Endpoint, GetProgress);
        builder.MapPost(UserContexts.SetProgress.Endpoint, SetLevelPassed);

        return builder;
    }

    private static async Task<UserContexts.GetProgress.Response> GetProgress(
        [FromBody] UserContexts.GetProgress.Request request,
        [FromServices] IGrainFactory grains)
    {
        var userProgress = grains.GetGrain<IUserProgress>(request.UserId);
        var state = await userProgress.GetProgress();

        var response = new UserContexts.GetProgress.Response
        {
            PassedLevels = state.PassedLevels
        };

        return response;
    }

    private static Task SetLevelPassed(
        [FromBody] UserContexts.SetProgress request,
        [FromServices] IGrainFactory grains)
    {
        var userProgress = grains.GetGrain<IUserProgress>(request.UserId);
        return userProgress.OnLevelPassed(request.Section, request.Level);
    }
}