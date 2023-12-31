

using API.CHAT;
using API.Data;
using API.Data.REPOSITORY;
using API.LIB.HELPERS;
using API.LIB.INTERFACES;
using API.LIB.SERVICES;
using Microsoft.EntityFrameworkCore;

namespace API.LIB.EXTENSIONS;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration){
        services.AddDbContext<DataContext>(options => {
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
        });

        // ## CORS
        services.AddCors();
        services.AddScoped<ITokenService, TokenService>();
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));
        services.AddScoped<IPhotoService, PhotoService>();
        services.AddScoped<LogUserActivity>();

        services.AddSignalR();
        services.AddSingleton<PresenceTracker>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}