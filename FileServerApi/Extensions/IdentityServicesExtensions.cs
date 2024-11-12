using FileServer.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace FileServerApi.Extensions
{
    public static class IdentityServicesExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services , IConfiguration configuration)
        {
            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
            }).AddEntityFrameworkStores<IdentityContext>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata=false;
                    options.SaveToken=false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateAudience=true,
                        ValidateIssuer=true,
                        ValidateLifetime=true,
                        ValidIssuer=configuration["JwtConfigurations:Issuer"],
                        ValidAudience=configuration["JwtConfigurations:Audience"],
                        IssuerSigningKey= new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtConfigurations:Key"]))

                    };
                });

            return services;
        }
    }
}
