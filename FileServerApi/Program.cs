
using FileServer.Infrastructure.Data;
using FileServer.Infrastructure.Identity;
using FileServerApi.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;

namespace FileServerApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddControllers()
               .AddNewtonsoftJson(options =>
               {
                   options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
               });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 50 * 1024 * 1024; // Limit to 50 MB
            });
            builder.Services.AddApplicationServices(builder.Configuration);
            builder.Services.AddIdentityServices(builder.Configuration);

            var app = builder.Build();

            #region Update Db
                
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;

            var LoggerFactoryService = services.GetRequiredService<ILoggerFactory>();
            try
            {
                var IdentityContext = services.GetRequiredService<IdentityContext>();
                var FileContext = services.GetRequiredService<FileContext>();

                await IdentityContext.Database.MigrateAsync();
                await FileContext.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                var logger = LoggerFactoryService.CreateLogger(typeof(Program));
                logger.LogError(ex, "An Error Occurred While Updating Database");
            }

            
            #endregion

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
