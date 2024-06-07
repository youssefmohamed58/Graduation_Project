using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Project.Core.Entities;
using Project.Repository.Identity;

namespace Project.Apis.Extensions
{
    public static class IdentityServicesExtension
    {
        public static IServiceCollection AddIdentityService(this IServiceCollection Services , IConfiguration configuration)
        {
            Services.AddIdentity<AppUser, IdentityRole>().AddDefaultTokenProviders()
                            .AddEntityFrameworkStores<AppIdentityDbContext>();
            Services.AddAuthentication(Options =>
            {
                Options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                Options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                                    .AddJwtBearer(Options =>
                                    {
                                        Options.TokenValidationParameters = new TokenValidationParameters()
                                        {
                                            ValidateIssuer = true,
                                            ValidIssuer = configuration["JWT:ValidIssuer"],
                                            ValidateAudience = true,
                                            ValidAudience = configuration["JWT:ValidAudiance"],
                                            ValidateLifetime = true,
                                            ValidateIssuerSigningKey = true,
                                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]))
                                        };
                                    });
            Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
            });


            return Services;
        }

    }
}
