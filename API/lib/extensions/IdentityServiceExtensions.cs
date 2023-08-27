
using System.Text;
using API.Data;
using API.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API.LIB.EXTENSIONS;

public static class IdentityServiceExtensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration){

        //NET IDENTITY
        services.AddIdentityCore<User>(options => {
            options.Password.RequireNonAlphanumeric = false;
        })
        .AddRoles<Role>()
        .AddRoleManager<RoleManager<Role>>()
        .AddEntityFrameworkStores<DataContext>();


        //JWT TOKEN
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(
                Options => {
                    Options.TokenValidationParameters = new TokenValidationParameters{
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["token_key"])),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                    };
                
                    //AUTHENTICATION FOR CHAT
                    Options.Events = new JwtBearerEvents{
                        OnMessageReceived = context => {
                            var accesToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;
                            if(!string.IsNullOrEmpty(accesToken) && path.StartsWithSegments("/hubs")){
                                context.Token = accesToken;
                            }

                            return Task.CompletedTask;
                        }
                    };
                }
            );

        services.AddAuthorization(opt => 
            {
                opt.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
                opt.AddPolicy("ModeratePhotoRole", policy => policy.RequireRole("Admin", "Moderator"));
            });

        return services;
    }
}