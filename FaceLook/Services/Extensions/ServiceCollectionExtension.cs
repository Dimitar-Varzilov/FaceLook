using FaceLook.Data;
using FaceLook.Services.MappingProfiles;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

namespace FaceLook.Services.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static void AddServices(this IServiceCollection services, IConfigurationManager configurationManager)
        {
            RegisterCustomServices(services);
            RegisterApplicationServices(services);
            RegisterDbContext(services, configurationManager);
            BindConfigurations(services, configurationManager);
        }

        private static void RegisterCustomServices(IServiceCollection services)
        {
            services.AddTransient<IEmailSender<IdentityUser>, EmailSender>();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<IUserService, UserService>();
        }

        private static void RegisterApplicationServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddHttpContextAccessor();
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<MesssagingProfile>();
            });
        }

        private static void RegisterDbContext(IServiceCollection services, IConfigurationManager configurationManager)
        {
            var connectionString = configurationManager.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
        }

        private static void BindConfigurations(IServiceCollection services, IConfigurationManager configurationManager)
        {
            services.Configure<MailServerOptions>(configurationManager.GetSection("MailServerOptions"));
        }
    }
}
