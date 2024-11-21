using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace API.Extentions;

public static class IdentityServiceExtentions
{
    public static IServiceCollection AddIdentityService(
            this IServiceCollection services,
            IConfiguration config)
    {

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
            });
        return services;
    }
}
