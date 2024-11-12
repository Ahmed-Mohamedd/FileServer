using FileServer.Core.Repositories_Interfaces;
using FileServer.Core.Services_Interfaces;
using FileServer.Infrastructure;
using FileServer.Infrastructure.Data;
using FileServer.Infrastructure.Identity;
using FileServer.Service;
using Microsoft.EntityFrameworkCore;

namespace FileServerApi.Extensions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<IdentityContext>(options =>
            {
                options.UseSqlite(configuration.GetConnectionString("IdentityConnection"));
            });
            services.AddDbContext<FileContext>(options =>
            {
                options.UseSqlite(configuration.GetConnectionString("FileServerConnection"));
            });


            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IFileService, FileService>();
            services.Configure<JwtConfigurations>(configuration.GetSection("JwtConfigurations"));

            return services;
        }
    }
}
