using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ChessBackend.Modules.Puzzles.Data;
using ChessBackend.Modules.Puzzles.Services;
using ChessBackend.Shared.Contracts.Puzzles;

namespace ChessBackend.Modules.Puzzles;

public static class PuzzlesModuleExtensions
{
    public static IServiceCollection AddPuzzlesModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PuzzlesDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<PuzzleService>();
        services.AddScoped<IPuzzleService>(sp => sp.GetRequiredService<PuzzleService>());
        services.AddScoped<IPuzzleModuleContract>(sp => sp.GetRequiredService<PuzzleService>());
        services.AddScoped<PuzzleSeeder>();

        return services;
    }
}
