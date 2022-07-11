using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingAppAPI.Data;
using DatingAppAPI.Interfaces;
using DatingAppAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace DatingAppAPI.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserService, UserService>();
            var connectionString = config.GetConnectionString("DefaultConnection");
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlite(connectionString);
            });
            return services;
        }
    }
}