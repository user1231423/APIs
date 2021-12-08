using Business.Files.Interfaces;
using Business.Files.Services;
using Business.Files.Settings;
using Data.Files;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Globalization;

namespace API.Files.Configuration
{
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
            services.AddSingleton<IFileSettings>(configuration.GetSection("FilesSettings").Get<FileSettings>());
        }

        /// <summary>
        /// Add settings
        /// </summary>
        /// <param name="services"></param>
        public static void AddServices(this IServiceCollection services)
        {
            services.AddSingleton<IFileService, FileService>();
        }

        /// <summary>
        /// Add db context
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<FilesDbContext>(options => options.UseNpgsql(configuration["ConnectionStrings:FilesConnectionString"]));
        }
    }
}
