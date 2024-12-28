using System;
using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Services;
using API.SignalR;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace API.Extentions;

public static class ApplicationServiceExtentions
{

    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
            IConfiguration config)
    {

        services.AddControllers();
        services.AddDbContext<DataContext>(opt =>
        {
            opt.UseSqlServer(config.GetConnectionString("DefaultConnection"));
            // opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
        });
        services.AddCors();

        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUserRepository, UserRepository>();
        //using auto mapper (all classes that implement Profile class)
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        services.Configure<CloudinarySettings>(config.GetSection("cloudinarySettings"));
        services.AddScoped<IPhotoService, PhotoService>();
        services.AddScoped<ILikesRepository, LikesRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        //To log the user activity after any changes / calling the apis
        services.AddScoped<LogUserActivity>();

        services.AddSignalR();
        services.AddSingleton<PresenceTracker>();

        return services;
    }
}
