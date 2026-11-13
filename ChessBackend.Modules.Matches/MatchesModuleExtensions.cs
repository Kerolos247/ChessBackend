using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ChessBackend.Modules.Matches.Data;
using ChessBackend.Modules.Matches.Services;
using ChessBackend.Shared.Contracts.Matches;

namespace ChessBackend.Modules.Matches;

public static class MatchesModuleExtensions
{
    public static IServiceCollection AddMatchesModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<MatchesDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<MatchService>();
        services.AddScoped<IMatchService>(sp => sp.GetRequiredService<MatchService>());
        services.AddScoped<IMatchModuleContract>(sp => sp.GetRequiredService<MatchService>());

        return services;
    }
}
