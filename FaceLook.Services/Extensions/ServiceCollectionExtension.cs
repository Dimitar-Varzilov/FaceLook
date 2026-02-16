using FaceLook.Data;
using FaceLook.Data.Entities;
using FaceLook.Services.Core;
using FaceLook.Services.Interfaces;
using FaceLook.Services.MappingProfiles;
using FaceLook.Services.Middlewares;
using FaceLook.Services.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;

namespace FaceLook.Services.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static void AddServices(this IServiceCollection services, IConfigurationManager configurationManager)
        {
            RegisterCustomServices(services);
            RegisterApplicationServices(services);
            RegisterApplicationMiddlewares(services);
            RegisterDbContext(services, configurationManager);
            BindConfigurations(services, configurationManager);
        }

        private static void RegisterCustomServices(IServiceCollection services)
        {
            services.AddScoped<IEmailSender<User>, EmailSender>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<IFileShareService, FileShareService>();
            services.AddScoped<IMessageService, MessageService>();
        }

        private static void RegisterApplicationServices(IServiceCollection services)
        {
            services.AddControllersWithViews().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
            services.AddHttpContextAccessor();
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<MesssagingProfile>();
            });
            services.AddSignalR();
        }

        private static void RegisterApplicationMiddlewares(IServiceCollection services)
        {
            services.AddTransient<ExceptionHandlingMiddleware>();
        }

        private static void RegisterDbContext(IServiceCollection services, IConfigurationManager configurationManager)
        {
            var connectionString = configurationManager.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
        }

        private static void BindConfigurations(IServiceCollection services, IConfigurationManager configurationManager)
        {
            services.Configure<MailServerOptions>(configurationManager.GetSection("MailServerOptions"));
            services.Configure<FileShareOptions>(configurationManager.GetSection("FileShareOptions"));
        }
    }
}
