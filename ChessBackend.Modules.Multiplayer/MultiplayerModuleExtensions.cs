using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using ChessBackend.Modules.Multiplayer.Hubs;
using ChessBackend.Modules.Multiplayer.Services;
using ChessBackend.Shared.Contracts.Multiplayer;

namespace ChessBackend.Modules.Multiplayer;

public static class MultiplayerModuleExtensions
{
    public static IServiceCollection AddMultiplayerModule(this IServiceCollection services)
    {
        services.AddSignalR();
        services.AddSingleton<RoomManager>();
        services.AddSingleton(typeof(ConnectionMapping<>));
        services.AddSingleton<IUserConnectionTracker, UserConnectionTracker>();

        return services;
    }

    public static IEndpointRouteBuilder MapMultiplayerModule(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHub<ChessHub>("/gameHub");
        return endpoints;
    }
}
