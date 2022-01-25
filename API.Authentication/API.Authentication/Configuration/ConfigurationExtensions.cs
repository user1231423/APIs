using Business.Authentication.Interfaces;
using Business.Authentication.Models;
using Business.Authentication.Services;
using Business.Authentication.Settings;
using Common.Data.Interfaces;
using Common.Data.Repositories;
using Common.Services.Interfaces;
using Common.Services.Services;
using Data.Authentication.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Globalization;

namespace API.Authentication.Configuration
{
    /// <summary>
    /// Configuration extensions
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Configure culture
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureCulture(this IServiceCollection services)
        {
            // Register cultures
            services.Configure<RequestLocalizationOptions>(
                opts =>
                {
                    var supportedCultures = new List<CultureInfo>
                    {
                        new CultureInfo("en_US"),
                        new CultureInfo("pt_PT")
                    };

                    opts.DefaultRequestCulture = new RequestCulture("en_US");
                    opts.SupportedCultures = supportedCultures;
                    opts.SupportedUICultures = supportedCultures;
                });
        }

        /// <summary>
        /// Add settings
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddSettings(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddSingleton<IJwtSettings>(configuration.GetSection("JwtSettings").Get<JwtSettings>());
		}

        /// <summary>
        /// Add settings
        /// </summary>
        /// <param name="services"></param>
		public static void AddServices(this IServiceCollection services)
		{
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUserService, UserService>();
			services.AddSingleton<IJwtService, JwtService>();
            services.AddSingleton<IConfigurationService, ConfigurationService>();
		}

        /// <summary>
        /// Add auto mapper
        /// </summary>
        /// <param name="services"></param>
        public static void AutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(Startup));
        }

        /// <summary>
        /// Add db context
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AuthenticationDbContext>(options => options.UseNpgsql(configuration["ConnectionStrings:AuthConnectionString"], b => b.MigrationsAssembly("API.Authentication")));
        }
    }
}
