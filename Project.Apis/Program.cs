using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project.Apis.Extensions;
using Project.Core.Entities;
using Project.Core.Interfaces;
using Project.Repository.Identity;
using Project.Services;

namespace Project.Apis
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<AppIdentityDbContext>(Options =>
            {
                Options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddIdentityService(builder.Configuration);
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder .Services.Configure<DataProtectionTokenProviderOptions>(opts => opts.TokenLifespan = TimeSpan.FromHours(10));
            builder.Services.AddCors();
            var app = builder.Build();

            #region Ubdate Database

            using var scope = app.Services.CreateScope();

            var services = scope.ServiceProvider;

            var identityDbContext = services.GetRequiredService<AppIdentityDbContext>();

            await identityDbContext.Database.MigrateAsync();

            var usermanager = services.GetRequiredService<UserManager<AppUser>>();
            await AppIdentityDbContextSeed.SeedUserAsync(usermanager);


            #endregion

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(); 
            }
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}