using DEVTOOLS.EF;
using INSPECTION.EF;
using INVENTORY.EF;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using REPOSITORY.EF;
using SOFTLAND.EF;
using System;
using System.Collections.Generic;
using System.Globalization;
using TCS.EF;
using TCS.WebUI.Data;
using TCS.WebUI.Interface;
using TCS.WebUI.Models;
using TCS.WebUI.Services;
using Wkhtmltopdf.NetCore;

namespace TCS.WebUI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ISoftlandService, SoftlandService>();

            services.AddDbContext<TCSContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("TCSCONN")));

            services.AddDbContext<INVENTORYRContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("INVENTORYCONN")));

            services.AddDbContext<REPOSITORYContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("REPOSITORYCONN")));

            services.AddDbContext<INSPECTIONContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("INSPECTIONCONN")));

            services.AddDbContext<SOFTLANDContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("SOFTLANDCONN")));

            services.AddDbContext<ALARMContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("ALARMCONN")));

            services.AddDbContext<DEVTOOLSContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DEVTOOLSCONN"),
            sqlServerOptionsAction: sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure();
            }
            ));

            services.AddScoped<Helpers.IAuthenticationService, Helpers.LdapAuthenticationService>();
            services.AddScoped<ITmsService, TmsService>();

            services.AddTransient<IMeetingService, MeetingService>();

            services.AddHttpContextAccessor();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.AddControllers();
            services.AddWkhtmltopdf();

            services.AddControllersWithViews().AddRazorRuntimeCompilation()
                .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

            services.Configure<SmartSettings>(Configuration.GetSection(SmartSettings.SectionName));

            // Note: This line is for demonstration purposes only, I would not recommend using this as a shorthand approach for accessing settings
            // While having to type '.Value' everywhere is driving me nuts (>_<), using this method means reloaded appSettings.json from disk will not work
            services.AddSingleton(s => s.GetRequiredService<IOptions<SmartSettings>>().Value);
            services.AddDatabaseDeveloperPageExceptionFilter();
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

            services.AddRazorPages();

            services.AddAuthentication(
            CookieAuthenticationDefaults.AuthenticationScheme
            ).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
                options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.LogoutPath = "/Account/Logout";
                    options.Cookie.Name = "auth0";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                });

            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddMvc()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization();
            services.AddScoped<IStringLocalizer, StringLocalizer<SharedResource>>();

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var cultures = new List<CultureInfo>
                {
                    new CultureInfo("es"),
                    new CultureInfo("en")
                    //new CultureInfo("fr")
                };
                options.DefaultRequestCulture = new RequestCulture("en");
                options.SupportedCultures = cultures;
                options.SupportedUICultures = cultures;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRequestLocalization(app.ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);
            app.UseSession();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    "default",
                    "{controller=Home}/{action=index}");
                endpoints.MapRazorPages();
            });

            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".ipa"] = "application/octet-stream";
            provider.Mappings[".plist"] = "txt/xml";
            provider.Mappings[".apk"] = "application/vnd.android.package-archive";

            app.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = provider
            });

            //app.UsePathBase("/test");
        }
    }
}
