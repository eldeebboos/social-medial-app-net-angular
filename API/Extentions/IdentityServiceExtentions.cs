using System;
using System.Text;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API.Extentions;

public static class IdentityServiceExtentions
{
    public static IServiceCollection AddIdentityService(
            this IServiceCollection services,
            IConfiguration config)
    {
        services.AddIdentityCore<AppUser>(opt =>
        {
            // opt.Password.RequireNonAlphanumeric=false;
        })
        .AddRoles<AppRole>()
        .AddRoleManager<RoleManager<AppRole>>()
        .AddEntityFrameworkStores<DataContext>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(option =>
            {
                var tokenKey = config["TokenKey"] ?? throw new Exception("Tokenkey not Found");
                option.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };

                //add authintication support in SignalR/Websocket 
                option.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        //use context.Request.Query in websocket instead of headers in http
                        var accessToken = context.Request.Query["access_token"];

                        var path = context.Request.Path;

                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
                // next step ==>need to adjust CORS configuration in program.cs
            });

        services.AddAuthorizationBuilder()
                    .AddPolicy("RequiredAdminRole", policy => policy.RequireRole("Admin"))
                    .AddPolicy("ModeratorPhotoRole", policy => policy.RequireRole("admin", "Moderator"));
        return services;
    }
}
